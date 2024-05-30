using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Models;

namespace Biblioteca.Interfaces
{
    public interface IEmprestimoService
    {
        /// <summary>
        /// Busca um empréstimo por ID.
        /// </summary>
        /// <param name="id">ID do empréstimo.</param>
        /// <returns>O empréstimo correspondente ao ID.</returns>
        Task<Emprestimo> BuscarPorId(int id);

        /// <summary>
        /// Busca todos os empréstimos.
        /// </summary>
        /// <returns>Lista de todos os empréstimos.</returns>
        Task<List<Emprestimo>> BuscarEmprestimos();

        /// <summary>
        /// Adiciona um novo empréstimo.
        /// </summary>
        /// <param name="emprestimo">Dados do empréstimo a ser adicionado.</param>
        /// <returns>O empréstimo recém-adicionado.</returns>
        Task<Emprestimo> Adicionar(Emprestimo emprestimo);

        /// <summary>
        /// Atualiza um empréstimo existente.
        /// </summary>
        /// <param name="emprestimo">Dados atualizados do empréstimo.</param>
        /// <param name="id">ID do empréstimo a ser atualizado.</param>
        /// <param name="funcionarioId">ID do funcionário que está atualizando o empréstimo.</param>
        /// <returns>O empréstimo atualizado.</returns>
        Task<Emprestimo> Atualizar(Emprestimo emprestimo, int id, int funcionarioId);

        /// <summary>
        /// Cria um novo empréstimo.
        /// </summary>
        /// <param name="emprestimoDto">Dados do empréstimo a ser criado.</param>
        /// <param name="funcionarioId">ID do funcionário que está criando o empréstimo.</param>
        /// <returns>O empréstimo recém-criado.</returns>
        Task<ReadEmprestimoDto> CreateEmprestimo(CreateEmprestimoDto emprestimoDto, int funcionarioId);

        /// <summary>
        /// Busca todos os empréstimos.
        /// </summary>
        /// <returns>Lista de todos os empréstimos.</returns>
        Task<IEnumerable<ReadEmprestimoDto>> GetEmprestimos();

        /// <summary>
        /// Busca um empréstimo por ID.
        /// </summary>
        /// <param name="id">ID do empréstimo.</param>
        /// <returns>O empréstimo correspondente ao ID.</returns>
        Task<ReadEmprestimoDto> GetEmprestimoById(int id);

        /// <summary>
        /// Busca todos os empréstimos de um usuário por ID do usuário.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Lista de todos os empréstimos do usuário.</returns>
        Task<IEnumerable<ReadEmprestimoDto>> GetEmprestimosByUsuarioId(int usuarioId);

        /// <summary>
        /// Devolve um empréstimo.
        /// </summary>
        /// <param name="usuarioId">ID do usuário que está devolvendo o empréstimo.</param>
        /// <param name="emprestimoId">ID do empréstimo a ser devolvido.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        Task DevolverEmprestimo(int funcionarioId, int usuarioId, int emprestimoId);

        /// <summary>
        /// Renova um empréstimo existente.
        /// </summary>
        /// <param name="renovacaoDto">Objeto contendo os dados de renovação do empréstimo.</param>
        /// <param name="emprestimoId">ID do empréstimo a ser renovado.</param>
        /// <param name="funcionarioId">ID do funcionário responsável pela renovação.</param>
        /// <returns>Retorna o empréstimo renovado.</returns>
        Task<Emprestimo> RenovarEmprestimo(Emprestimo renovacaoDto, int emprestimoId, int funcionarioId);
    }
}
