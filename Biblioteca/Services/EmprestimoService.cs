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

            // Calcula o número de dias de atraso
            var diasDeAtraso = (DateTime.UtcNow - emprestimoPorId.DataPrevistaDevolucao).Days;

            if (diasDeAtraso > 0)
            {
                // Calcula o valor da multa (R$1 por dia de atraso)
                var valorMulta = diasDeAtraso;

                // Verifica se a multa excede o dobro do valor do livro
                if (valorMulta >= (2 * emprestimoPorId.Livro.Valor))
                {
                    // Marca o empréstimo como travado automaticamente
                    emprestimoPorId.Status = StatusEmprestimo.Atrasado;
                }

                // Adiciona a nova multa ao banco de dados
                emprestimoPorId.Multas.Add(new Multa { Valor = valorMulta });
            }

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

        public async Task<ReadEmprestimoDto> CreateEmprestimo(CreateEmprestimoDto emprestimoDto, int funcionarioId)
        {
            // Verifica se o usuário existe
            var usuario = await _usuarioService.BuscarPorId(emprestimoDto.UsuarioId);
            if (usuario == null)
            {
                throw new ArgumentException($"Usuário com ID {emprestimoDto.UsuarioId} não encontrado.");
            }

            // Verifica se o exemplar existe e está disponível
            var exemplar = await _dbContext.Exemplares
                .Include(e => e.Livro)
                .FirstOrDefaultAsync(e => e.ExemplarId == emprestimoDto.ExemplarId);
            if (exemplar == null || exemplar.Status != StatusExemplar.Disponivel)
            {
                throw new InvalidOperationException("Exemplar não disponível para empréstimo.");
            }

            // Verifica o número de empréstimos ativos do usuário
            int numEmprestimosAtivos = await _dbContext.Emprestimos
                .CountAsync(e => e.UsuarioId == emprestimoDto.UsuarioId &&
                    (e.Status == StatusEmprestimo.EmAndamento || e.Status == StatusEmprestimo.Atrasado));

            // Lança exceção se o limite for excedido
            if (numEmprestimosAtivos >= 3)
            {
                throw new InvalidOperationException("Limite de empréstimos ativos do usuário atingido.");
            }

            // Verifica histórico de atrasos
            bool hasLateReturns = await _dbContext.Emprestimos
                .AnyAsync(e => e.UsuarioId == emprestimoDto.UsuarioId && e.Status == StatusEmprestimo.Atrasado);
            if (hasLateReturns)
            {
                throw new InvalidOperationException("Usuário tem histórico de atrasos.");
            }

            // Verifica se o usuário está bloqueado
            if (usuario.Bloqueado)
            {
                throw new InvalidOperationException("Usuário está bloqueado.");
            }

            // Cria o empréstimo
            var emprestimo = _mapper.Map<Emprestimo>(emprestimoDto);
            emprestimo.FuncionarioId = funcionarioId;
            emprestimo.Status = StatusEmprestimo.EmAndamento;
            emprestimo.DataEmprestimo = DateTime.Now;
            emprestimo.LivroId = exemplar.LivroId;

            // Calcula a data prevista de devolução
            emprestimo.DataPrevistaDevolucao = CalcularDataPrevistaDevolucao(emprestimo.DataEmprestimo);

            try
            {
                await _dbContext.Emprestimos.AddAsync(emprestimo);
                exemplar.Status = StatusExemplar.Emprestado; // Atualiza o status do exemplar para "emprestado"
                _dbContext.Exemplares.Update(exemplar); // Marca o exemplar como modificado para que a alteração de status seja salva
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Log de erro detalhado para entender a causa raiz
                Console.Error.WriteLine(dbEx.InnerException?.Message ?? dbEx.Message);
                throw;
            }

            return _mapper.Map<ReadEmprestimoDto>(emprestimo);
        }
        private DateTime CalcularDataPrevistaDevolucao(DateTime dataEmprestimo)
        {
            // Adiciona 7 dias úteis à data de empréstimo
            var dataPrevista = dataEmprestimo;
            int diasUteis = 0;

            while (diasUteis < 7)
            {
                dataPrevista = dataPrevista.AddDays(1);

                // Verifica se o dia é um dia útil (segunda a sexta)
                if (dataPrevista.DayOfWeek != DayOfWeek.Saturday && dataPrevista.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasUteis++;
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
        public async Task DevolverEmprestimo(int funcionarioId, int usuarioId, int emprestimoId)
        {
            // Verifica se o funcionário existe e tem permissão
            var funcionario = await _dbContext.Funcionarios.FindAsync(funcionarioId);
            if (funcionario == null)
            {
                throw new ArgumentException($"Funcionário com ID {funcionarioId} não encontrado.");
            }

            // Verifica se o empréstimo existe
            var emprestimo = await _dbContext.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                    .ThenInclude(ex => ex.Livro)
                .FirstOrDefaultAsync(e => e.EmprestimoId == emprestimoId);

            if (emprestimo == null)
            {
                throw new ArgumentException($"Empréstimo com ID {emprestimoId} não encontrado.");
            }

            // Verifica se o usuário tem permissão para devolver o empréstimo
            if (emprestimo.UsuarioId != usuarioId)
            {
                throw new InvalidOperationException("Usuário não tem permissão para devolver este empréstimo.");
            }

            // Verifica se o empréstimo está em andamento
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

                // Calcula o valor da multa
                float valorMulta = diasAtraso * 1.00f; // Adiciona R$1,00 de multa para cada dia de atraso

                // Se a multa atingir o dobro do valor do livro, mantém a multa no valor máximo
                float valorMaximoMulta = 2 * emprestimo.Exemplar.Livro.Valor;
                if (valorMulta > valorMaximoMulta)
                {
                    valorMulta = valorMaximoMulta;
                }

                // Cria a multa
                var multa = new Multa
                {
                    EmprestimoId = emprestimoId,
                    Valor = valorMulta,
                    InicioMulta = dataDevolucao,
                    DiasAtrasados = diasAtraso,
                    Status = StatusMulta.Pendente // Define o status da multa como pendente
                };

                // Adiciona a multa ao contexto
                await _dbContext.Multas.AddAsync(multa);

                // Se a multa atingir o dobro do valor do livro, bloqueia o usuário
                if (valorMulta == valorMaximoMulta)
                {
                    emprestimo.Usuario.Bloqueado = true;
                    _dbContext.Usuarios.Update(emprestimo.Usuario);
                }
            }

            // Atualiza o status do exemplar para disponível
            emprestimo.Exemplar.Status = StatusExemplar.Disponivel;
            _dbContext.Exemplares.Update(emprestimo.Exemplar);

            // Atualiza o status do empréstimo
            emprestimo.Status = StatusEmprestimo.Devolvido;
            _dbContext.Emprestimos.Update(emprestimo);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw new InvalidOperationException("Erro ao processar a devolução do empréstimo.", ex);
            }
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

