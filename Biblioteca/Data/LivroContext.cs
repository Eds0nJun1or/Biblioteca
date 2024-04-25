using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Data
{
    public class LivroContext : DbContext
    {
        public LivroContext(DbContextOptions<LivroContext> opts)
            : base(opts)
        {

        }

        public DbSet<livro> Livros { get; set; }
    }
}
