using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Services;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExemplarController : ControllerBase
    {
        private readonly IExemplarService _exemplarService;

        public ExemplarController(IExemplarService exemplarService)
        {
            _exemplarService = exemplarService;
        }

        /// <summary>
        /// Cria um novo exemplar com base nos dados fornecidos.
        /// </summary>
        /// <param name="exemplarDto">Objeto DTO contendo os dados do exemplar a ser criado.</param>
        /// <returns>Retorna uma mensagem de sucesso se o exemplar for criado.</returns>
        /// <response code="201">Retorna uma mensagem de sucesso e a URL para acessá-lo.</response>
        /// <response code="400">Retorna mensagem de erro se os dados fornecidos forem inválidos.</response>
        /// <response code="404">Retorna mensagem de erro se o livro associado não for encontrado.</response>
        /// <response code="500">Retorna mensagem de erro se ocorrer um erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateExemplar(CreateExemplarDto exemplarDto)
        {
            try
            {
                _exemplarService.CreateExemplar(exemplarDto);
                return CreatedAtAction(nameof(GetExemplaresByLivroId), new { id = exemplarDto.LivroId }, "Exemplar criado com sucesso.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor.");
            }
        }

        /// <summary>
        /// Busca todos os exemplares.
        /// </summary>
        /// <returns>Retorna uma lista de exemplares.</returns>
        /// <response code="200">Retorna uma lista de exemplares.</response>
        /// <response code="500">Retorna mensagem de erro se ocorrer um erro interno no servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReadExemplarDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllExemplares()
        {
            try
            {
                var exemplares = _exemplarService.GetAllExemplares();
                return Ok(exemplares);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor.");
            }
        }

        /// <summary>
        /// Busca exemplares por ID do livro.
        /// </summary>
        /// <param name="id">ID do livro.</param>
        /// <returns>Retorna uma lista de exemplares associados ao ID do livro.</returns>
        /// <response code="200">Retorna uma lista de exemplares associados ao ID do livro.</response>
        /// <response code="404">Retorna mensagem de erro se não houver exemplares para o ID do livro.</response>
        /// <response code="500">Retorna mensagem de erro se ocorrer um erro interno no servidor.</response>
        [HttpGet("{id}/exemplares")]
        [ProducesResponseType(typeof(IEnumerable<ReadExemplarDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetExemplaresByLivroId(int id)
        {
            try
            {
                var exemplares = _exemplarService.GetExemplaresByLivroId(id);
                if (!exemplares.Any())
                {
                    return NotFound("Nenhum exemplar encontrado para o ID do livro fornecido.");
                }
                return Ok(exemplares);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor.");
            }
        }
    }
}
