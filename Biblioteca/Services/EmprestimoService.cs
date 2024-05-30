using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Enums;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Interfaces;
using Biblioteca.Services.Exceptions;

namespace Biblioteca.Services
{
    public class EmprestimoService : IEmprestimoService
    {
        private readonly BibliotecaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _config;

        public EmprestimoService(BibliotecaContext dbContext, IMapper mapper, IUsuarioService usuarioService, IConfiguration config)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _usuarioService = usuarioService;
            _config = config;
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

        public async Task<Emprestimo> Atualizar(Emprestimo emprestimo, int emprestimoId, int funcionarioId)
        {
            var emprestimoPorId = await _dbContext.Emprestimos
                .Include(e => e.Exemplar)
                    .ThenInclude(ex => ex.Livro)
                .Include(e => e.Multas)
                .FirstOrDefaultAsync(e => e.EmprestimoId == emprestimoId);

            if (emprestimoPorId == null)
            {
                throw new EmprestimoNotFoundException($"Empréstimo com ID {emprestimoId} não encontrado.");
            }

            // Verifica se há atraso
            if (emprestimoPorId.DataDevolucao.HasValue && emprestimoPorId.DataDevolucao.Value > emprestimoPorId.DataPrevistaDevolucao)
            {
                var diasDeAtraso = (emprestimoPorId.DataDevolucao.Value - emprestimoPorId.DataPrevistaDevolucao).Days;
                var valorMulta = diasDeAtraso * 1.00f;

                // Verifica se o valor da multa ultrapassa o limite
                var valorMaximoMulta = 2 * emprestimoPorId.Exemplar.Livro.Valor;
                if (valorMulta >= valorMaximoMulta)
                {
                    emprestimoPorId.Status = StatusEmprestimo.Atrasado;
                    valorMulta = valorMaximoMulta; // Define o valor da multa como o máximo permitido
                }

                // Verifica se já existe uma multa associada a este empréstimo
                var multaExistente = emprestimoPorId.Multas.FirstOrDefault();
                if (multaExistente != null)
                {
                    // Atualiza o valor da multa existente
                    multaExistente.Valor = valorMulta;
                }
                else
                {
                    // Adiciona uma nova multa ao empréstimo
                    emprestimoPorId.Multas.Add(new Multa { Valor = valorMulta });
                }
            }

            // Atualiza os campos do empréstimo existente com os valores do empréstimo fornecido
            emprestimoPorId.DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao;
            emprestimoPorId.DataDevolucao = emprestimo.DataDevolucao;
            emprestimoPorId.Status = emprestimo.Status;

            // Atualiza a data de renovação se o status for Renovado
            if (emprestimo.Status == StatusEmprestimo.Renovado)
            {
                // Use o método GetValue<T> para obter o valor da configuração
                emprestimoPorId.DataPrevistaDevolucao = DateTime.Now.AddDays(_config.GetValue<int>("DiasRenovacao"));
            }

            // Salva as alterações no banco de dados
            await _dbContext.SaveChangesAsync();

            return emprestimoPorId;
        }

        public async Task<Emprestimo> RenovarEmprestimo(Emprestimo renovacaoDto, int emprestimoId, int funcionarioId)
        {
            var emprestimoPorId = await _dbContext.Emprestimos
                .Include(e => e.Exemplar)
                    .ThenInclude(ex => ex.Livro)
                .Include(e => e.Multas)
                .FirstOrDefaultAsync(e => e.EmprestimoId == emprestimoId);

            if (emprestimoPorId == null)
            {
                throw new EmprestimoNotFoundException($"Empréstimo com ID {emprestimoId} não encontrado.");
            }

            // Atualiza os campos do empréstimo existente com os valores da renovação
            emprestimoPorId.DataDevolucao = renovacaoDto.DataDevolucao;
            emprestimoPorId.Status = renovacaoDto.Status;

            // Atualiza a data de renovação se o status for Renovado
            if (renovacaoDto.Status == StatusEmprestimo.Renovado)
            {
                // Use o método GetValue<T> para obter o valor da configuração
                emprestimoPorId.DataPrevistaDevolucao = DateTime.Now.AddDays(_config.GetValue<int>("DiasRenovacao"));
            }

            // Salva as alterações no banco de dados
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

            bool multasPendentes = usuario.Emprestimos.Any(e => e.Multas.Any(m => m.Status == StatusMulta.Pendente));

            if (multasPendentes && usuario.Emprestimos.Count(e => e.DataDevolucao == null) >= 1)
            {
                throw new InvalidOperationException("Usuário com multas pendentes só pode pegar um livro por vez.");
            }

            int numEmprestimosAtivos = usuario.Emprestimos.Count(e => e.DataDevolucao == null);

            if (numEmprestimosAtivos >= 3)
            {
                throw new InvalidOperationException("Limite de empréstimos ativos do usuário atingido.");
            }

            bool historicoAtrasos = usuario.Emprestimos.Any(e => e.Status == StatusEmprestimo.Atrasado);

            if (historicoAtrasos)
            {
                throw new InvalidOperationException("Usuário tem histórico de atrasos.");
            }

            var emprestimo = _mapper.Map<Emprestimo>(emprestimoDto);
            emprestimo.FuncionarioId = funcionarioId;
            emprestimo.Status = StatusEmprestimo.EmAndamento;
            emprestimo.DataEmprestimo = DateTime.Now; // Define a data do empréstimo como a data atual
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
                .Include(e => e.Multas) // Carregar Multas explicitamente
                .ToListAsync();

            var emprestimosDto = new List<ReadEmprestimoDto>();

            foreach (var emprestimo in emprestimos)
            {
                var emprestimoDto = _mapper.Map<ReadEmprestimoDto>(emprestimo);

                emprestimoDto.NomeUsuario = emprestimo.Usuario.Nome;
                emprestimoDto.TituloExemplar = emprestimo.Exemplar.Livro.Nome;
                emprestimoDto.Multa = emprestimo.Multas != null && emprestimo.Multas.Any(m => m.Status == StatusMulta.Pendente);

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
                .Include(e => e.Multas) // Carregar Multas explicitamente
                .FirstOrDefaultAsync(e => e.EmprestimoId == id);

            if (emprestimo == null)
            {
                return null;
            }

            var emprestimoDto = _mapper.Map<ReadEmprestimoDto>(emprestimo);

            emprestimoDto.NomeUsuario = emprestimo.Usuario.Nome;
            emprestimoDto.TituloExemplar = emprestimo.Exemplar.Livro.Nome;
            emprestimoDto.Multa = emprestimo.Multas != null && emprestimo.Multas.Any(m => m.Status == StatusMulta.Pendente);

            return emprestimoDto;
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

            // Atualiza o status do empréstimo para "Devolvido"
            emprestimo.DataDevolucao = DateTime.Now;
            emprestimo.Status = StatusEmprestimo.Devolvido;
            emprestimo.FuncionarioId = funcionarioId;

            // Atualiza o status do exemplar para "Disponível"
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
