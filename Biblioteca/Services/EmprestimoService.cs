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
    /// <summary>
    /// Serviço para operações relacionadas a empréstimos.
    /// </summary>
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

        /// <summary>
        /// Busca um empréstimo pelo ID.
        /// </summary>
        /// <param name="id">ID do empréstimo.</param>
        /// <returns>Retorna o objeto do empréstimo.</returns>
        public async Task<Emprestimo> BuscarPorId(int id)
        {
            return await _dbContext.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                .ThenInclude(ex => ex.Livro)
                .FirstOrDefaultAsync(e => e.EmprestimoId == id);
        }

        /// <summary>
        /// Busca todos os empréstimos.
        /// </summary>
        /// <returns>Retorna uma lista de objetos de empréstimo.</returns>
        public async Task<List<Emprestimo>> BuscarEmprestimos()
        {
            return await _dbContext.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                .ThenInclude(ex => ex.Livro)
                .ToListAsync();
        }

        /// <summary>
        /// Adiciona um novo empréstimo.
        /// </summary>
        /// <param name="emprestimo">Objeto do empréstimo a ser adicionado.</param>
        /// <returns>Retorna o objeto do empréstimo adicionado.</returns>
        public async Task<Emprestimo> Adicionar(Emprestimo emprestimo)
        {
            await _dbContext.Emprestimos.AddAsync(emprestimo);
            await _dbContext.SaveChangesAsync();

            return emprestimo;
        }

        /// <summary>
        /// Atualiza um empréstimo existente.
        /// </summary>
        /// <param name="emprestimo">Objeto do empréstimo com os novos dados.</param>
        /// <param name="id">ID do empréstimo a ser atualizado.</param>
        /// <param name="funcionarioId">ID do funcionário responsável pela atualização.</param>
        /// <returns>Retorna o objeto do empréstimo atualizado.</returns>
        public async Task<Emprestimo> Atualizar(Emprestimo emprestimo, int id, int funcionarioId)
        {
            var emprestimoPorId = await BuscarPorId(id);
            if (emprestimoPorId == null)
            {
                throw new ArgumentException($"Empréstimo para o ID: {id} não foi encontrado no banco de dados.");
            }

            emprestimoPorId.ExemplarId = emprestimo.ExemplarId;
            emprestimoPorId.UsuarioId = emprestimo.UsuarioId;
            emprestimoPorId.FuncionarioId = emprestimo.FuncionarioId;
            emprestimoPorId.DataEmprestimo = emprestimo.DataEmprestimo;
            emprestimoPorId.DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao;
            emprestimoPorId.DataDevolucao = emprestimo.DataDevolucao;
            emprestimoPorId.Status = emprestimo.Status;

            _dbContext.Emprestimos.Update(emprestimoPorId);
            await _dbContext.SaveChangesAsync();

            return emprestimoPorId;
        }

        /// <summary>
        /// Verifica se há multa associada a um empréstimo.
        /// </summary>
        /// <param name="emprestimoId">ID do empréstimo.</param>
        /// <returns>Retorna true se houver multa, senão false.</returns>
        private async Task<bool> VerificarMulta(int emprestimoId)
        {
            var multa = await _dbContext.Multas.FirstOrDefaultAsync(m => m.EmprestimoId == emprestimoId && m.Status == StatusMulta.Pendente);
            return multa != null;
        }

        /// <summary>
        /// Cria um novo empréstimo com base nos dados fornecidos.
        /// Verifica se o livro e o usuário associados ao empréstimo existem no banco de dados.
        /// Verifica se o usuário já atingiu o limite de empréstimos ou tem histórico de atrasos.
        /// Se o usuário não estiver bloqueado e atender aos critérios de empréstimo, um novo empréstimo é criado.
        /// </summary>
        /// <param name="emprestimoDto">Objeto DTO contendo os dados do empréstimo a ser criado.</param>
        /// <returns>Retorna o objeto DTO do empréstimo criado.</returns>
        public async Task<ReadEmprestimoDto> CreateEmprestimo(CreateEmprestimoDto emprestimoDto, int funcionarioId)
        {
            var usuario = await _usuarioService.BuscarPorId(emprestimoDto.UsuarioId);

            if (usuario == null)
            {
                throw new ArgumentException($"Usuário com ID {emprestimoDto.UsuarioId} não encontrado.");
            }

            var livro = await _dbContext.Livros.FindAsync(emprestimoDto.ExemplarId);
            if (livro == null)
            {
                throw new ArgumentException($"Exemplar com ID {emprestimoDto.ExemplarId} não encontrado.");
            }

            int numEmprestimosUsuario = await _dbContext.Emprestimos.CountAsync(e => e.UsuarioId == emprestimoDto.UsuarioId && (e.Status == StatusEmprestimo.EmAndamento || e.Status == StatusEmprestimo.Atrasado));
            if (numEmprestimosUsuario >= 3)
            {
                throw new InvalidOperationException("Limite de empréstimos do usuário atingido.");
            }

            bool hasLateReturns = await _dbContext.Emprestimos.AnyAsync(e => e.UsuarioId == emprestimoDto.UsuarioId && e.Status == StatusEmprestimo.Atrasado);
            if (hasLateReturns)
            {
                throw new InvalidOperationException("Usuário tem histórico de atrasos.");
            }

            var exemplarDisponivel = await _dbContext.Exemplares.FirstOrDefaultAsync(e => e.ExemplarId == emprestimoDto.ExemplarId && e.Status == StatusExemplar.Disponivel);
            if (exemplarDisponivel == null)
            {
                throw new InvalidOperationException("Exemplar não disponível para empréstimo.");
            }

            if (usuario.Bloqueado)
            {
                throw new InvalidOperationException("Usuário está bloqueado.");
            }

            var emprestimo = _mapper.Map<Emprestimo>(emprestimoDto);
            emprestimo.FuncionarioId = funcionarioId;
            emprestimo.Status = StatusEmprestimo.EmAndamento;
            emprestimo.DataEmprestimo = DateTime.Now;

            // Calcula a data prevista de devolução como 7 dias úteis a partir da data de empréstimo
            emprestimo.DataPrevistaDevolucao = CalcularDataPrevistaDevolucao(emprestimo.DataEmprestimo);

            await _dbContext.Emprestimos.AddAsync(emprestimo);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<ReadEmprestimoDto>(emprestimo);
        }

        /// <summary>
        /// Calcula a data prevista de devolução como 7 dias úteis a partir da data de empréstimo.
        /// </summary>
        /// <param name="dataEmprestimo">Data de empréstimo.</param>
        /// <returns>Data prevista de devolução.</returns>
        private DateTime CalcularDataPrevistaDevolucao(DateTime dataEmprestimo)
        {
            const int diasUteis = 7;
            int diasContados = 0;
            DateTime dataPrevista = dataEmprestimo.AddDays(1); // Começa no próximo dia útil

            while (diasContados < diasUteis)
            {
                dataPrevista = dataPrevista.AddDays(1);
                if (dataPrevista.DayOfWeek != DayOfWeek.Saturday && dataPrevista.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasContados++;
                }
            }

            return dataPrevista;
        }

        /// <summary>
        /// Recupera todos os empréstimos.
        /// </summary>
        /// <returns>Retorna uma lista de objetos DTO de empréstimos.</returns>
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

                // Obter nome do usuário
                emprestimoDto.NomeUsuario = emprestimo.Usuario.Nome;

                // Obter título do exemplar
                emprestimoDto.TituloExemplar = emprestimo.Exemplar.Livro.Nome;

                // Verificar se há multa associada ao empréstimo
                emprestimoDto.Multa = await VerificarMulta(emprestimo.EmprestimoId);

                emprestimosDto.Add(emprestimoDto);
            }

            return emprestimosDto;
        }


        /// <summary>
        /// Recupera um empréstimo pelo ID.
        /// </summary>
        /// <param name="id">ID do empréstimo.</param>
        /// <returns>Retorna o objeto DTO do empréstimo.</returns>
        public async Task<ReadEmprestimoDto> GetEmprestimoById(int id)
        {
            var emprestimo = await BuscarPorId(id);
            if (emprestimo == null)
            {
                throw new EmprestimoNotFoundException($"Empréstimo com ID {id} não encontrado.");
            }

            var readEmprestimoDto = _mapper.Map<ReadEmprestimoDto>(emprestimo);

            // Obter nome do usuário
            readEmprestimoDto.NomeUsuario = emprestimo.Usuario.Nome;

            // Obter título do exemplar
            readEmprestimoDto.TituloExemplar = emprestimo.Exemplar.Livro.Nome;

            // Verificar se há multa associada ao empréstimo
            readEmprestimoDto.Multa = await VerificarMulta(emprestimo.EmprestimoId);

            return readEmprestimoDto;
        }

        /// <summary>
        /// Devolve um empréstimo.
        /// Calcula multas se houver atrasos e bloqueia o usuário se a multa for maior ou igual ao dobro do valor do livro.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <param name="emprestimoId">ID do empréstimo.</param>
        /// <returns>Tarefa assíncrona.</returns>
        public async Task DevolverEmprestimo(int usuarioId, int emprestimoId)
        {
            var emprestimo = await _dbContext.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                    .ThenInclude(ex => ex.Livro)
                .FirstOrDefaultAsync(e => e.EmprestimoId == emprestimoId);

            if (emprestimo == null)
            {
                throw new ArgumentException($"Empréstimo com ID {emprestimoId} não encontrado.");
            }

            if (emprestimo.UsuarioId != usuarioId)
            {
                throw new InvalidOperationException("Usuário não tem permissão para devolver este empréstimo.");
            }

            if (emprestimo.Status != StatusEmprestimo.EmAndamento)
            {
                throw new InvalidOperationException("Este empréstimo não está mais em andamento.");
            }

            DateTime dataDevolucao = DateTime.Now;
            emprestimo.DataDevolucao = dataDevolucao;

            // Verifica se a devolução está atrasada
            if (dataDevolucao > emprestimo.DataPrevistaDevolucao)
            {
                // Calcula o número de dias de atraso
                int diasAtraso = (int)(dataDevolucao - emprestimo.DataPrevistaDevolucao).TotalDays;

                // Cria a multa
                var multa = new Multa
                {
                    EmprestimoId = emprestimoId,
                    Valor = diasAtraso * 1.00f, // Adiciona R$1,00 de multa para cada dia de atraso
                    InicioMulta = dataDevolucao,
                    DiasAtrasados = diasAtraso,
                    Status = StatusMulta.Pendente // Define o status da multa como pendente
                };

                // Adiciona a multa ao contexto
                await _dbContext.Multas.AddAsync(multa);

                // Se a multa atingir o dobro do valor do livro, bloqueia o usuário
                if (multa.Valor >= (2 * emprestimo.Livro.Valor))
                {
                    emprestimo.Usuario.Bloqueado = true;
                    _dbContext.Usuarios.Update(emprestimo.Usuario);
                }
            }

            emprestimo.Status = StatusEmprestimo.Devolvido;
            _dbContext.Emprestimos.Update(emprestimo);
            await _dbContext.SaveChangesAsync();
        }


        /// <summary>
        /// Recupera empréstimos de um usuário pelo ID do usuário.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Retorna uma lista de objetos DTO de empréstimos do usuário.</returns>
        public async Task<IEnumerable<ReadEmprestimoDto>> GetEmprestimosByUsuarioId(int usuarioId)
        {
            var emprestimos = await _dbContext.Emprestimos
                .Where(e => e.UsuarioId == usuarioId)
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                .ThenInclude(ex => ex.Livro)
                .ToListAsync();

            if (!emprestimos.Any())
            {
                throw new UsuarioNotFoundException("Usuário não possui empréstimos.");
            }

            var readEmprestimoDtos = _mapper.Map<List<ReadEmprestimoDto>>(emprestimos);

            foreach (var readEmprestimoDto in readEmprestimoDtos)
            {
                var emprestimo = emprestimos.First(e => e.EmprestimoId == readEmprestimoDto.EmprestimoId);

                // Obter nome do usuário
                readEmprestimoDto.NomeUsuario = emprestimo.Usuario.Nome;

                // Obter título do exemplar
                readEmprestimoDto.TituloExemplar = emprestimo.Exemplar.Livro.Nome;

                // Verificar se há multa associada ao empréstimo
                readEmprestimoDto.Multa = await VerificarMulta(emprestimo.EmprestimoId);
            }

            return readEmprestimoDtos;
        }
    }
}

