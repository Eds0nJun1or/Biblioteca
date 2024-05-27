using Microsoft.AspNetCore.Mvc;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Exceptions;
using Biblioteca.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteca.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MultaController : ControllerBase
    {
        private readonly IMultaService _multaService;

        public MultaController(IMultaService multaService)
        {
            _multaService = multaService;
        }

        /// <summary>
        /// Adiciona uma multa ao banco de dados.
        /// </summary>
        /// <param name="multaDto">Objeto com os campos necessários para criação de uma multa.</param>
        /// <returns>IActionResult</returns>
        /// <response code="201">Caso a inserção seja feita com sucesso.</response>
        /// <response code="400">Caso o pedido seja inválido.</response>
        /// <response code="404">Caso o empréstimo não seja encontrado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdicionaMulta([FromBody] CreateMultaDto multaDto)
        {
            try
            {
                var multaId = await _multaService.AdicionarMulta(multaDto);
                return CreatedAtAction(nameof(RecuperaMultaPorId), new { id = multaId }, multaDto);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Recupera uma multa pelo ID.
        /// </summary>
        /// <param name="id">ID da multa.</param>
        /// <returns>IActionResult</returns>
        /// <response code="200">Caso a multa seja encontrada.</response>
        /// <response code="404">Caso a multa não seja encontrada.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RecuperaMultaPorId(int id)
        {
            try
            {
                var multa = await _multaService.RecuperarMultaPorId(id);
                return Ok(multa);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Recupera todas as multas.
        /// </summary>
        /// <returns>IActionResult</returns>
        /// <response code="200">Caso as multas sejam encontradas.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RecuperaMultas()
        {
            var multas = await _multaService.RecuperarTodasAsMultas();
            return Ok(multas);
        }

        /// <summary>
        /// Paga uma multa.
        /// </summary>
        /// <param name="id">ID da multa.</param>
        /// <returns>IActionResult</returns>
        /// <response code="200">Caso a multa seja paga com sucesso.</response>
        /// <response code="404">Caso a multa não seja encontrada.</response>
        /// <response code="400">Caso a multa já tenha sido paga.</response>
        [HttpPost("Pagar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PagarMulta(int id)
        {
            try
            {
                await _multaService.PagarMulta(id);
                return Ok("Multa paga com sucesso.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
