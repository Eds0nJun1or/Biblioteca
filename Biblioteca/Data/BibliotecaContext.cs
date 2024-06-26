﻿using Biblioteca.Enums;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;
namespace Biblioteca.Data
{
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> opts)
            : base(opts)
        {

        }

        public DbSet<Livro> Livros { get; set; }
        public DbSet<Exemplar> Exemplares { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Emprestimo> Emprestimos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Multa> Multas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Emprestimo>()
              .HasOne(e => e.Exemplar)
              .WithMany(e => e.Emprestimos);

            modelBuilder.Entity<Emprestimo>()
              .HasOne(e => e.Usuario)
              .WithMany(u => u.Emprestimos);

            modelBuilder.Entity<Exemplar>()
              .HasOne(e => e.Livro)
              .WithMany(l => l.Exemplares);

            modelBuilder.Entity<Multa>()
              .HasOne(m => m.Emprestimo)
              .WithMany(e => e.Multas)
              .HasForeignKey(m => m.EmprestimoId);

            // Conversão de enum para string no banco de dados
            modelBuilder.Entity<Emprestimo>()
              .Property(e => e.Status)
              .HasConversion<string>(
                v => v.ToString(),
                v => Enum.Parse<StatusEmprestimo>(v));

            modelBuilder.Entity<Exemplar>()
                .Property(e => e.Status)
                .HasConversion<string>(
                    v => v.ToString(),
                    v => Enum.Parse<StatusExemplar>(v));
        }
    }
}
