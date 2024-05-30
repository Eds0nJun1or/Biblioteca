using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;

namespace Biblioteca.Services
{
    /// <summary>
    /// Interface para o serviço de exemplares.
    /// </summary>
    public interface IExemplarService
    {
        /// <summary>
        /// Cria um novo exemplar com base nos dados fornecidos.
        /// </summary>
        /// <param name="exemplarDto">Objeto DTO contendo os dados do exemplar a ser criado.</param>
        void CreateExemplar(CreateExemplarDto exemplarDto);

        /// <summary>
        /// Retorna todos os exemplares.
        /// </summary>
        /// <returns>Uma coleção de DTOs de leitura de exemplares.</returns>
        IEnumerable<ReadExemplarDto> GetAllExemplares();

        /// <summary>
        /// Retorna os exemplares associados a um ID de livro específico.
        /// </summary>
        /// <param name="livroId">ID do livro para o qual buscar os exemplares.</param>
        /// <returns>Uma coleção de DTOs de leitura de exemplares.</returns>
        IEnumerable<ReadExemplarDto> GetExemplaresByLivroId(int livroId);
    }
}
