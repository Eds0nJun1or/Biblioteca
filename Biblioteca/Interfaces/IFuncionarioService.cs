using Biblioteca.Models;

namespace Biblioteca.Interfaces
{
    /// <summary>
    /// Interface para operações relacionadas a funcionários.
    /// </summary>
    public interface IFuncionarioService
    {
        /// <summary>
        /// Busca um funcionário pelo ID.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário.</param>
        /// <returns>Retorna o objeto do funcionário encontrado, ou null se não for encontrado.</returns>
        Task<Funcionario> BuscarFuncionarioPorIdAsync(int funcionarioId);

        /// <summary>
        /// Busca todos os funcionários.
        /// </summary>
        /// <returns>Retorna uma lista de objetos de funcionários.</returns>
        Task<IEnumerable<Funcionario>> BuscarTodosFuncionariosAsync();

        /// <summary>
        /// Cria um novo funcionário.
        /// </summary>
        /// <param name="funcionario">Objeto do funcionário a ser criado.</param>
        /// <returns>Retorna o objeto do funcionário criado.</returns>
        Task<Funcionario> CriarFuncionarioAsync(Funcionario funcionario);

        /// <summary>
        /// Atualiza um funcionário existente.
        /// </summary>
        /// <param name="funcionario">Objeto do funcionário com os novos dados.</param>
        Task AtualizarFuncionarioAsync(Funcionario funcionario);

        /// <summary>
        /// Exclui um funcionário pelo ID.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário a ser excluído.</param>
        Task ExcluirFuncionarioAsync(int funcionarioId);
    }
}
