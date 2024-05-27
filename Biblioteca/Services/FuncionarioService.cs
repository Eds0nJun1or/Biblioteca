using Biblioteca.Data;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Services
{
    public class FuncionarioService : IFuncionarioService
    {
        private readonly BibliotecaContext _context;

        public FuncionarioService(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<Funcionario> BuscarFuncionarioPorIdAsync(int funcionarioId)
        {
            return await _context.Funcionarios.FindAsync(funcionarioId);
        }

        public async Task<IEnumerable<Funcionario>> BuscarTodosFuncionariosAsync()
        {
            return await _context.Funcionarios.ToListAsync();
        }

        public async Task<Funcionario> CriarFuncionarioAsync(Funcionario funcionario)
        {
            await _context.Funcionarios.AddAsync(funcionario);
            await _context.SaveChangesAsync();

            return funcionario;
        }

        public async Task AtualizarFuncionarioAsync(Funcionario funcionario)
        {
            _context.Funcionarios.Update(funcionario);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirFuncionarioAsync(int funcionarioId)
        {
            var funcionario = await _context.Funcionarios.FindAsync(funcionarioId);
            if (funcionario != null)
            {
                _context.Funcionarios.Remove(funcionario);
                await _context.SaveChangesAsync();
            }
        }
    }
}
