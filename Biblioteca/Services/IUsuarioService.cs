using Biblioteca.Models;

namespace Biblioteca.Services
{
    public interface IUsuarioService
    {
        Task<List<Usuario>> BuscarUsuarios();
        Task<Usuario> BuscarPorId(int id);
        Task<Usuario> Adicionar(Usuario usuario);
        Task<Usuario> Atualizar(Usuario usuario, int id);
        Task<bool> Apagar(int id);
    }
}
