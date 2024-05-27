using Biblioteca.Data;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Services
{
    /// <summary>
    /// Implementação do serviço para operações relacionadas a livros.
    /// </summary>
    public class LivroService : ILivroService
    {
        private readonly BibliotecaContext _dbContext;

        /// <summary>
        /// Construtor do serviço LivroService.
        /// </summary>
        /// <param name="bibliotecaContext">Contexto do banco de dados.</param>
        public LivroService(BibliotecaContext bibliotecaContext)
        {
            _dbContext = bibliotecaContext;
        }

        /// <summary>
        /// Busca um livro pelo ID.
        /// </summary>
        /// <param name="id">ID do livro.</param>
        /// <returns>Retorna o objeto do livro encontrado, ou null se não for encontrado.</returns>
        public async Task<Livro> BuscarPorId(int id)
        {
            return await _dbContext.Livros.FirstOrDefaultAsync(x => x.LivroId == id);
        }

        /// <summary>
        /// Busca todos os livros.
        /// </summary>
        /// <returns>Retorna uma lista de objetos de livros.</returns>
        public async Task<List<Livro>> BuscarLivros()
        {
            return await _dbContext.Livros.ToListAsync();
        }

        /// <summary>
        /// Adiciona um novo livro.
        /// </summary>
        /// <param name="livro">Objeto do livro a ser adicionado.</param>
        /// <returns>Retorna o objeto do livro adicionado.</returns>
        public async Task<Livro> Adicionar(Livro livro)
        {
            await _dbContext.Livros.AddAsync(livro);
            await _dbContext.SaveChangesAsync();
            return livro;
        }

        /// <summary>
        /// Atualiza um livro existente.
        /// </summary>
        /// <param name="livro">Objeto do livro com os novos dados.</param>
        /// <param name="id">ID do livro a ser atualizado.</param>
        /// <returns>Retorna o objeto do livro atualizado.</returns>
        public async Task<Livro> Atualizar(Livro livro, int id)
        {
            Livro livroPorId = await BuscarPorId(id);
            if (livroPorId == null)
            {
                throw new ArgumentException($"Livro para o ID: {id} não foi encontrado no banco de dados.");
            }

            livroPorId.Nome = livro.Nome;
            livroPorId.Autor = livro.Autor;
            livroPorId.Editora = livro.Editora; 
            livroPorId.DataPublicacao = livro.DataPublicacao; 
            livroPorId.Genero = livro.Genero;
            livroPorId.Paginas = livro.Paginas;
            livroPorId.Valor = livro.Valor;
            livroPorId.Status = livro.Status;

            await _dbContext.SaveChangesAsync();
            return livroPorId;
        }

        /// <summary>
        /// Apaga um livro pelo ID.
        /// </summary>
        /// <param name="id">ID do livro a ser apagado.</param>
        /// <returns>Retorna true se o livro foi apagado com sucesso.</returns>
        public async Task<bool> Apagar(int id)
        {
            Livro livroPorId = await BuscarPorId(id);
            if (livroPorId == null)
            {
                throw new ArgumentException($"Livro para o ID: {id} não foi encontrado no banco de dados.");
            }

            _dbContext.Livros.Remove(livroPorId);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Pesquisa livros com base nos parâmetros fornecidos.
        /// </summary>
        /// <param name="titulo">Título do livro (obrigatório).</param>
        /// <param name="autor">Autor do livro (opcional).</param>
        /// <param name="categoria">Categoria do livro (opcional).</param>
        /// <returns>Retorna uma lista de objetos de livros encontrados com base nos parâmetros fornecidos.</returns>
        public async Task<List<Livro>> PesquisarLivros(string titulo, string autor = null, string categoria = null)
        {
            var query = _dbContext.Livros.AsQueryable();

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(livro => livro.Nome.Contains(titulo));
            }

            if (!string.IsNullOrEmpty(autor))
            {
                query = query.Where(livro => livro.Autor.Contains(autor));
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(livro => livro.Genero.Contains(categoria));
            }

            return await query.ToListAsync();
        }
    }
}
