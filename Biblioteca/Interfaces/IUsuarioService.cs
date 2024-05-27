using Biblioteca.Models;

namespace Biblioteca.Interfaces
{
    /// <summary>
    /// Interface para operações relacionadas a usuários.
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Busca todos os usuários.
        /// </summary>
        /// <returns>Retorna uma lista de objetos de usuários.</returns>
        Task<List<Usuario>> BuscarUsuarios();

        /// <summary>
        /// Busca um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Retorna o objeto do usuário encontrado, ou null se não for encontrado.</returns>
        Task<Usuario> BuscarPorId(int id);

        /// <summary>
        /// Adiciona um novo usuário.
        /// </summary>
        /// <param name="usuario">Objeto do usuário a ser adicionado.</param>
        /// <returns>Retorna o objeto do usuário adicionado.</returns>
        Task<Usuario> Adicionar(Usuario usuario);

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="usuario">Objeto do usuário com os novos dados.</param>
        /// <param name="id">ID do usuário a ser atualizado.</param>
        /// <returns>Retorna o objeto do usuário atualizado.</returns>
        Task<Usuario> Atualizar(Usuario usuario, int id);

        /// <summary>
        /// Apaga um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário a ser apagado.</param>
        /// <returns>Retorna true se o usuário foi apagado com sucesso.</returns>
        Task<bool> Apagar(int id);
    }
}
