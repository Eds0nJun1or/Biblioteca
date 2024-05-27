using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;

namespace Biblioteca.Interfaces
{
    /// <summary>
    /// Interface para operações relacionadas a multas.
    /// </summary>
    public interface IMultaService
    {
        /// <summary>
        /// Adiciona uma nova multa.
        /// </summary>
        /// <param name="multaDto">Objeto com os dados para criação da multa.</param>
        /// <returns>Retorna o ID da multa criada.</returns>
        Task<int> AdicionarMulta(CreateMultaDto multaDto);

        /// <summary>
        /// Recupera uma multa pelo ID.
        /// </summary>
        /// <param name="multaId">ID da multa.</param>
        /// <returns>Retorna o objeto da multa encontrada.</returns>
        Task<ReadMultaDto> RecuperarMultaPorId(int multaId);

        /// <summary>
        /// Atualiza uma multa existente.
        /// </summary>
        /// <param name="multaId">ID da multa.</param>
        /// <param name="multaDto">Objeto com os dados atualizados da multa.</param>
        Task AtualizarMulta(int multaId, UpdateMultaDto multaDto);

        /// <summary>
        /// Recupera todas as multas.
        /// </summary>
        /// <returns>Retorna uma lista de objetos de multas.</returns>
        Task<IEnumerable<ReadMultaDto>> RecuperarTodasAsMultas();

        /// <summary>
        /// Paga uma multa pelo ID.
        /// </summary>
        /// <param name="multaId">ID da multa.</param>
        Task PagarMulta(int multaId);
    }
}

