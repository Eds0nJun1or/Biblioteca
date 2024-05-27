using Biblioteca.Models;

namespace Biblioteca.Interfaces
{
    /// <summary>
    /// Interface para operações relacionadas a livros.
    /// </summary>
    public interface ILivroService
    {
        /// <summary>
        /// Busca todos os livros.
        /// </summary>
        /// <returns>Retorna uma lista de objetos de livros.</returns>
        Task<List<Livro>> BuscarLivros();

        /// <summary>
        /// Busca um livro pelo ID.
        /// </summary>
        /// <param name="id">ID do livro.</param>
        /// <returns>Retorna o objeto do livro encontrado, ou null se não for encontrado.</returns>
        Task<Livro> BuscarPorId(int id);

        /// <summary>
        /// Adiciona um novo livro.
        /// </summary>
        /// <param name="livro">Objeto do livro a ser adicionado.</param>
        /// <returns>Retorna o objeto do livro adicionado.</returns>
        Task<Livro> Adicionar(Livro livro);

        /// <summary>
        /// Atualiza um livro existente.
        /// </summary>
        /// <param name="livro">Objeto do livro com os novos dados.</param>
        /// <param name="id">ID do livro a ser atualizado.</param>
        /// <returns>Retorna o objeto do livro atualizado.</returns>
        Task<Livro> Atualizar(Livro livro, int id);

        /// <summary>
        /// Apaga um livro pelo ID.
        /// </summary>
        /// <param name="id">ID do livro a ser apagado.</param>
        /// <returns>Retorna true se o livro foi apagado com sucesso.</returns>
        Task<bool> Apagar(int id);

        /// <summary>
        /// Pesquisa livros com base nos parâmetros fornecidos.
        /// </summary>
        /// <param name="titulo">Título do livro (obrigatório).</param>
        /// <param name="autor">Autor do livro (opcional).</param>
        /// <param name="categoria">Categoria do livro (opcional).</param>
        /// <returns>Retorna uma lista de objetos de livros encontrados com base nos parâmetros fornecidos.</returns>
        Task<List<Livro>> PesquisarLivros(string titulo, string autor = null, string categoria = null);
    }
}
