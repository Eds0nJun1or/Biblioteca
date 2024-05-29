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
            return await _context.Funcionarios
                .AsNoTracking()
                .Select(f => new Funcionario
                {
                    FuncionarioId = f.FuncionarioId,
                    NomeFuncionario = f.NomeFuncionario,
                    DataNascimento = f.DataNascimento,
                    Email = f.Email,
                    Telefone = f.Telefone,
                    Status = f.Status
                })
                .FirstOrDefaultAsync(f => f.FuncionarioId == funcionarioId);
        }

        public async Task<IEnumerable<Funcionario>> BuscarTodosFuncionariosAsync()
        {
            return await _context.Funcionarios
                .AsNoTracking()
                .Select(f => new Funcionario
                {
                    FuncionarioId = f.FuncionarioId,
                    NomeFuncionario = f.NomeFuncionario,
                    DataNascimento = f.DataNascimento,
                    Email = f.Email,
                    Telefone = f.Telefone,
                    Status = f.Status
                })
                .ToListAsync();
        }

        public async Task<Funcionario> CriarFuncionarioAsync(Funcionario funcionario)
        {
            // Verificar duplicação de CPF
            if (_context.Funcionarios.Any(f => f.Cpf == funcionario.Cpf))
            {
                throw new InvalidOperationException("Já existe um funcionário com este CPF.");
            }

            // Verificar duplicação de senha
            if (_context.Funcionarios.Any(f => f.Senha == funcionario.Senha))
            {
                throw new InvalidOperationException("Já existe um funcionário com esta senha.");
            }

            // Verificar duplicação de e-mail
            if (_context.Funcionarios.Any(f => f.Email == funcionario.Email))
            {
                throw new InvalidOperationException("Já existe um funcionário com este e-mail.");
            }

            // Verificar duplicação de telefone
            if (_context.Funcionarios.Any(f => f.Telefone == funcionario.Telefone))
            {
                throw new InvalidOperationException("Já existe um funcionário com este telefone.");
            }

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
