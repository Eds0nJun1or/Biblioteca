using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Enums;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Interfaces;

namespace Biblioteca.Services
{
    public class EmprestimoService : IEmprestimoService
    {
        private readonly BibliotecaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;

        public EmprestimoService(BibliotecaContext dbContext, IMapper mapper, IUsuarioService usuarioService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _usuarioService = usuarioService;
        }

        public async Task<Emprestimo> BuscarPorId(int id)
        {
            return await _dbContext.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                .ThenInclude(ex => ex.Livro)
                .FirstOrDefaultAsync(e => e.EmprestimoId == id);
        }

        public async Task<List<Emprestimo>> BuscarEmprestimos()
        {
            return await _dbContext.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                .ThenInclude(ex => ex.Livro)
                .ToListAsync();
        }

        public async Task<Emprestimo> Adicionar(Emprestimo emprestimo)
        {
            await _dbContext.Emprestimos.AddAsync(emprestimo);
            await _dbContext.SaveChangesAsync();
            return emprestimo;
        }

        public async Task<Emprestimo> Atualizar(Emprestimo emprestimo, int id, int funcionarioId)
        {
            var emprestimoPorId = await BuscarPorId(id);
            if (emprestimoPorId == null)
            {
                throw new ArgumentException($"Empréstimo para o ID: {id} não foi encontrado no banco de dados.");
            }

            emprestimoPorId.ExemplarId = emprestimo.ExemplarId;
            emprestimoPorId.UsuarioId = emprestimo.UsuarioId;
            emprestimoPorId.FuncionarioId = funcionarioId;
            emprestimoPorId.DataEmprestimo = emprestimo.DataEmprestimo;
            emprestimoPorId.DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao;
            emprestimoPorId.DataDevolucao = emprestimo.DataDevolucao;
            emprestimoPorId.Status = emprestimo.Status;

            if (emprestimoPorId.DataDevolucao.HasValue && emprestimoPorId.DataDevolucao.Value > emprestimoPorId.DataPrevistaDevolucao)
            {
                var diasDeAtraso = (emprestimoPorId.DataDevolucao.Value - emprestimoPorId.DataPrevistaDevolucao).Days;
                var valorMulta = diasDeAtraso * 1.00f;

                if (valorMulta >= (2 * emprestimoPorId.Exemplar.Livro.Valor))
                {
                    emprestimoPorId.Status = StatusEmprestimo.Atrasado;
                }

                emprestimoPorId.Multas.Add(new Multa { Valor = valorMulta });
            }

            _dbContext.Emprestimos.Update(emprestimoPorId);
            await _dbContext.SaveChangesAsync();

            return emprestimoPorId;
        }

        private async Task<bool> VerificarMulta(int emprestimoId)
        {
            var multa = await _dbContext.Multas.FirstOrDefaultAsync(m => m.EmprestimoId == emprestimoId && m.Status == StatusMulta.Pendente);
            return multa != null;
        }

        public async Task<ReadEmprestimoDto> CreateEmprestimo(CreateEmprestimoDto emprestimoDto, int funcionarioId)
        {
            var usuario = await _dbContext.Usuarios
                .Include(u => u.Emprestimos)
                .ThenInclude(e => e.Multas)
                .FirstOrDefaultAsync(u => u.UsuarioId == emprestimoDto.UsuarioId);

            if (usuario == null)
            {
                throw new ArgumentException($"Usuário com ID {emprestimoDto.UsuarioId} não encontrado.");
            }

            var exemplar = await _dbContext.Exemplares
                .Include(e => e.Livro)
                .FirstOrDefaultAsync(e => e.ExemplarId == emprestimoDto.ExemplarId);

            if (exemplar == null || exemplar.Status != StatusExemplar.Disponivel)
            {
                throw new InvalidOperationException("Exemplar não disponível para empréstimo.");
            }

            bool hasPendingMultas = usuario.Emprestimos.Any(e => e.Multas.Any(m => m.Status == StatusMulta.Pendente));

            if (hasPendingMultas && usuario.Emprestimos.Count(e => e.DataDevolucao == null) >= 1)
            {
                throw new InvalidOperationException("Usuário com multas pendentes só pode pegar um livro por vez.");
            }

            int numEmprestimosAtivos = usuario.Emprestimos.Count(e => e.DataDevolucao == null);

            if (numEmprestimosAtivos >= 3)
            {
                throw new InvalidOperationException("Limite de empréstimos ativos do usuário atingido.");
            }

            bool hasLateReturns = usuario.Emprestimos.Any(e => e.Status == StatusEmprestimo.Atrasado);

            if (hasLateReturns)
            {
                throw new InvalidOperationException("Usuário tem histórico de atrasos.");
            }

            var emprestimo = _mapper.Map<Emprestimo>(emprestimoDto);
            emprestimo.FuncionarioId = funcionarioId;
            emprestimo.Status = StatusEmprestimo.EmAndamento;
            emprestimo.DataEmprestimo = DateTime.Now;
            emprestimo.LivroId = exemplar.LivroId;

            emprestimo.DataPrevistaDevolucao = CalcularDataPrevistaDevolucao(emprestimo.DataEmprestimo);

            try
            {
                await _dbContext.Emprestimos.AddAsync(emprestimo);
                exemplar.Status = StatusExemplar.Emprestado;
                _dbContext.Exemplares.Update(exemplar);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine(dbEx.InnerException?.Message ?? dbEx.Message);
                throw;
            }

            return _mapper.Map<ReadEmprestimoDto>(emprestimo);
        }

        private DateTime CalcularDataPrevistaDevolucao(DateTime dataEmprestimo)
        {
            var dataPrevista = dataEmprestimo;
            int diasUteis = 0;

            while (diasUteis < 7)
            {
                dataPrevista = dataPrevista.AddDays(1);

                if (dataPrevista.DayOfWeek != DayOfWeek.Saturday && dataPrevista.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasUteis++;
                }
            }

            return dataPrevista;
        }

        public async Task<IEnumerable<ReadEmprestimoDto>> GetEmprestimos()
        {
            var emprestimos = await _dbContext.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                    .ThenInclude(ex => ex.Livro)
                .ToListAsync();

            var emprestimosDto = new List<ReadEmprestimoDto>();

            foreach (var emprestimo in emprestimos)
            {
                var emprestimoDto = _mapper.Map<ReadEmprestimoDto>(emprestimo);

                emprestimoDto.NomeUsuario = emprestimo.Usuario.Nome;
                emprestimoDto.TituloExemplar = emprestimo.Exemplar.Livro.Nome;
                emprestimoDto.Multa = await VerificarMulta(emprestimo.EmprestimoId);

                emprestimosDto.Add(emprestimoDto);
            }

            return emprestimosDto;
        }

        public async Task<ReadEmprestimoDto> GetEmprestimoById(int id)
        {
            var emprestimo = await _dbContext.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                    .ThenInclude(ex => ex.Livro)
                .FirstOrDefaultAsync(e => e.EmprestimoId == id);

            if (emprestimo == null)
            {
                return null;
            }

            var emprestimoDto = _mapper.Map<ReadEmprestimoDto>(emprestimo);

            emprestimoDto.NomeUsuario = emprestimo.Usuario.Nome;
            emprestimoDto.TituloExemplar = emprestimo.Exemplar.Livro.Nome;
            emprestimoDto.Multa = await VerificarMulta(emprestimo.EmprestimoId);

            return emprestimoDto;
        }

        public async Task<bool> DeleteEmprestimo(int id)
        {
            var emprestimo = await _dbContext.Emprestimos.FindAsync(id);

            if (emprestimo == null)
            {
                return false;
            }

            _dbContext.Emprestimos.Remove(emprestimo);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> FinalizarEmprestimo(int id)
        {
            var emprestimo = await BuscarPorId(id);

            if (emprestimo == null)
            {
                throw new ArgumentException($"Empréstimo para o ID: {id} não foi encontrado no banco de dados.");
            }

            if (await VerificarMulta(id))
            {
                throw new InvalidOperationException("Empréstimo possui multas pendentes.");
            }

            emprestimo.DataDevolucao = DateTime.Now;
            emprestimo.Status = StatusEmprestimo.Devolvido;

            _dbContext.Emprestimos.Update(emprestimo);

            var exemplar = await _dbContext.Exemplares.FirstOrDefaultAsync(e => e.ExemplarId == emprestimo.ExemplarId);

            if (exemplar != null)
            {
                exemplar.Status = StatusExemplar.Disponivel;
                _dbContext.Exemplares.Update(exemplar);
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ReadEmprestimoDto>> GetEmprestimosByUsuarioId(int usuarioId)
        {
            var emprestimos = await _dbContext.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                    .ThenInclude(ex => ex.Livro)
                .Where(e => e.UsuarioId == usuarioId)
                .ToListAsync();

            if (!emprestimos.Any())
            {
                throw new ArgumentException($"Não foram encontrados empréstimos para o usuário com ID {usuarioId}.");
            }

            var emprestimosDto = emprestimos.Select(e => {
                var emprestimoDto = _mapper.Map<ReadEmprestimoDto>(e);
                emprestimoDto.NomeUsuario = e.Usuario.Nome;
                emprestimoDto.TituloExemplar = e.Exemplar.Livro.Nome;
                emprestimoDto.Multa = VerificarMulta(e.EmprestimoId).Result;
                return emprestimoDto;
            }).ToList();

            return emprestimosDto;
        }

        public async Task DevolverEmprestimo(int funcionarioId, int usuarioId, int emprestimoId)
        {
            var emprestimo = await _dbContext.Emprestimos
                .Include(e => e.Exemplar)
                .FirstOrDefaultAsync(e => e.EmprestimoId == emprestimoId && e.UsuarioId == usuarioId);

            if (emprestimo == null)
            {
                throw new ArgumentException($"Empréstimo com ID {emprestimoId} para o usuário com ID {usuarioId} não encontrado.");
            }

            if (emprestimo.Status == StatusEmprestimo.Devolvido)
            {
                throw new InvalidOperationException("Empréstimo já foi devolvido.");
            }

            if (await VerificarMulta(emprestimoId))
            {
                throw new InvalidOperationException("Empréstimo possui multas pendentes.");
            }

            emprestimo.DataDevolucao = DateTime.Now;
            emprestimo.Status = StatusEmprestimo.Devolvido;
            emprestimo.FuncionarioId = funcionarioId;

            var exemplar = await _dbContext.Exemplares.FirstOrDefaultAsync(e => e.ExemplarId == emprestimo.ExemplarId);
            if (exemplar != null)
            {
                exemplar.Status = StatusExemplar.Disponivel;
                _dbContext.Exemplares.Update(exemplar);
            }

            _dbContext.Emprestimos.Update(emprestimo);
            await _dbContext.SaveChangesAsync();
        }
    }
}
