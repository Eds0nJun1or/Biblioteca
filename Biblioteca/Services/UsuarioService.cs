using Biblioteca.Data;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly BibliotecaContext _dbContext;
        public UsuarioService(BibliotecaContext bibliotecaContext)
        {
            _dbContext = bibliotecaContext;
        }

        public async Task<Usuario> BuscarPorId(int id)
        {
            return await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.UsuarioId == id);
        }

        public async Task<List<Usuario>> BuscarUsuarios()
        {
            return await _dbContext.Usuarios.ToListAsync();
        }

        public async Task<Usuario> Adicionar(Usuario usuario)
        {
            // Verifica se já existe um usuário com o mesmo CPF
            if (_dbContext.Usuarios.Any(u => u.Cpf == usuario.Cpf))
            {
                throw new InvalidOperationException("Já existe um usuário com este CPF.");
            }

            // Verifica se já existe um usuário com o mesmo e-mail
            if (_dbContext.Usuarios.Any(u => u.Email == usuario.Email))
            {
                throw new InvalidOperationException("Já existe um usuário com este e-mail.");
            }

            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario> Atualizar(Usuario usuario, int id)
        {
            Usuario usuarioPorId = await BuscarPorId(id);
            if (usuarioPorId == null)
            {
                throw new ArgumentException($"Usuário para o ID: {id} não foi encontrado no banco de dados.");
            }

            usuarioPorId.Nome = usuario.Nome;
            usuarioPorId.Cpf = usuario.Cpf;
            usuarioPorId.Email = usuario.Email;

            _dbContext.Usuarios.Update(usuarioPorId);
            await _dbContext.SaveChangesAsync();

            return usuarioPorId;
        }

        public async Task<bool> Apagar(int id)
        {
            Usuario usuarioPorId = await BuscarPorId(id);
            if (usuarioPorId == null)
            {
                throw new ArgumentException($"Usuário para o ID: {id} não foi encontrado no banco de dados.");
            }

            _dbContext.Usuarios.Remove(usuarioPorId);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
