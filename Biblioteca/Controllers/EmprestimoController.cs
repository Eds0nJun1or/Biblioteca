using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Interfaces;
using Biblioteca.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace Biblioteca.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EmprestimoController : ControllerBase
    {
        private readonly IEmprestimoService _emprestimoService;

        public EmprestimoController(IEmprestimoService emprestimoService)
        {
            _emprestimoService = emprestimoService;
        }

        /// <summary>
        /// Cria um novo empréstimo com base nos dados fornecidos.
        /// </summary>
        /// <param name="emprestimoDto">Objeto DTO contendo os dados do empréstimo a ser criado.</param>
        /// <param name="funcionarioId">ID do funcionário responsável pela criação do empréstimo.</param>
        /// <returns>Retorna o objeto de empréstimo criado.</returns>
        /// <response code="201">Retorna o objeto de empréstimo criado e a URL para acessá-lo.</response>
        /// <response code="404">Retorna mensagem de erro se o empréstimo não for encontrado.</response>
        /// <response code="400">Retorna mensagem de erro se o limite de empréstimos for excedido ou se o exemplar não estiver disponível.</response>
        /// <response code="500">Retorna mensagem de erro se ocorrer um erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ReadEmprestimoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReadEmprestimoDto>> CreateEmprestimo(CreateEmprestimoDto emprestimoDto, int funcionarioId)
        {
            try
            {
                var novoEmprestimo = await _emprestimoService.CreateEmprestimo(emprestimoDto, funcionarioId);
                return CreatedAtAction(nameof(GetEmprestimoById), new { id = novoEmprestimo.EmprestimoId }, novoEmprestimo);
            }
            catch (EmprestimoNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Obtém uma lista de todos os empréstimos.
        /// </summary>
        /// <returns>Retorna uma lista de objetos de empréstimo.</returns>
        /// <response code="200">Retorna a lista de empréstimos.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReadEmprestimoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReadEmprestimoDto>>> GetEmprestimos()
        {
            var emprestimos = await _emprestimoService.GetEmprestimos();
            return Ok(emprestimos);
        }

        /// <summary>
        /// Obtém um empréstimo específico pelo ID.
        /// </summary>
        /// <param name="id">ID do empréstimo a ser obtido.</param>
        /// <returns>Retorna o objeto de empréstimo solicitado.</returns>
        /// <response code="200">Retorna o objeto de empréstimo solicitado.</response>
        /// <response code="404">Retorna mensagem de erro se o empréstimo não for encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReadEmprestimoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReadEmprestimoDto>> GetEmprestimoById(int id)
        {
            try
            {
                var emprestimo = await _emprestimoService.GetEmprestimoById(id);
                if (emprestimo == null)
                {
                    return NotFound();
                }
                return Ok(emprestimo);
            }
            catch (EmprestimoNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Obtém todos os empréstimos de um usuário específico.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Retorna uma lista de objetos de empréstimo do usuário.</returns>
        /// <response code="200">Retorna a lista de empréstimos do usuário.</response>
        /// <response code="404">Retorna mensagem de erro se o usuário não tiver empréstimos.</response>
        [HttpGet("LivrosEmprestados/{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<ReadEmprestimoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ReadEmprestimoDto>>> GetEmprestimosByUsuarioId(int usuarioId)
        {
            try
            {
                var emprestimos = await _emprestimoService.GetEmprestimosByUsuarioId(usuarioId);
                return Ok(emprestimos);
            }
            catch (UsuarioNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um empréstimo existente.
        /// </summary>
        /// <param name="id">ID do empréstimo a ser atualizado.</param>
        /// <param name="emprestimoDto">Objeto contendo os dados atualizados do empréstimo.</param>
        /// <param name="funcionarioId">ID do funcionário responsável pela atualização.</param>
        /// <returns>Retorna NoContent se a atualização for bem-sucedida.</returns>
        /// <response code="204">Indica que a atualização foi bem-sucedida.</response>
        /// <response code="400">Retorna mensagem de erro se o ID do empréstimo não corresponder ao ID fornecido.</response>
        /// <response code="404">Retorna mensagem de erro se o empréstimo não for encontrado.</response>
        /// <response code="500">Retorna mensagem de erro se ocorrer um erro interno no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEmprestimo(int id, UpdateEmprestimoDto emprestimoDto, int funcionarioId, IMapper _mapper)
        {
            if (id != emprestimoDto.EmprestimoId)
            {
                return BadRequest("ID do empréstimo não corresponde ao ID fornecido.");
            }

            try
            {
                var emprestimo = _mapper.Map<Emprestimo>(emprestimoDto);
                await _emprestimoService.Atualizar(emprestimo, id, funcionarioId);

                // Mapeia o objeto atualizado para o novo DTO de resposta
                var responseDto = new UpdateEmprestimoDto
                {
                    DataPrevistaDevolucao = emprestimoDto.DataPrevistaDevolucao,
                    DataDevolucao = emprestimoDto.DataDevolucao,
                    Status = emprestimoDto.Status
                };

                // Retorna o novo DTO de resposta
                return Ok(responseDto);
            }
            catch (EmprestimoNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Renova um empréstimo existente.
        /// </summary>
        /// <param name="id">ID do empréstimo a ser renovado.</param>
        /// <param name="renovacaoDto">Objeto contendo os dados de renovação do empréstimo.</param>
        /// <param name="funcionarioId">ID do funcionário responsável pela renovação.</param>
        /// <returns>Retorna NoContent se a renovação for bem-sucedida.</returns>
        /// <response code="204">Indica que a renovação foi bem-sucedida.</response>
        /// <response code="400">Retorna mensagem de erro se o ID do empréstimo não corresponder ao ID fornecido.</response>
        /// <response code="404">Retorna mensagem de erro se o empréstimo não for encontrado.</response>
        /// <response code="500">Retorna mensagem de erro se ocorrer um erro interno no servidor.</response>
        [HttpPut("{id}/renovacao")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RenovarEmprestimo(int id, UpdateRenovacaoDto renovacaoDto, int funcionarioId, IMapper _mapper)
        {
            if (id != renovacaoDto.EmprestimoId)
            {
                return BadRequest("ID do empréstimo não corresponde ao ID fornecido.");
            }

            try
            {
                var emprestimo = _mapper.Map<Emprestimo>(renovacaoDto);
                await _emprestimoService.RenovarEmprestimo(emprestimo, id, funcionarioId);

                return NoContent();
            }
            catch (EmprestimoNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Devolve um empréstimo pelo ID do funcionário.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário.</param>
        /// <param name="emprestimoId">ID do empréstimo a ser devolvido.</param>
        /// <returns>Retorna NoContent se a devolução for bem-sucedida.</returns>
        /// <response code="204">Indica que a devolução foi bem-sucedida.</response>
        /// <response code="404">Retorna mensagem de erro se o empréstimo ou usuário não for encontrado.</response>
        /// <response code="400">Retorna mensagem de erro se o empréstimo já estiver devolvido.</response>
        /// <response code="500">Retorna mensagem de erro se ocorrer um erro interno no servidor.</response>
        [HttpPost("devolver/{funcionarioId}/{usuarioId}/{emprestimoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DevolverEmprestimo(int funcionarioId, int usuarioId, int emprestimoId)
        {
            try
            {
                await _emprestimoService.DevolverEmprestimo(funcionarioId, usuarioId, emprestimoId);
                return NoContent();
            }
            catch (EmprestimoNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (EmprestimoJaDevolvidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UsuarioSemPermissaoException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
