using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioRepository;

        public UsuarioController(IUsuarioService usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        /// <summary>
        /// Cadastra um novo usuário.
        /// </summary>
        /// <param name="usuario">Objeto contendo os dados do usuário.</param>
        /// <returns>O usuário cadastrado.</returns>
        /// <response code="201">Caso o usuário seja cadastrado com sucesso.</response>
        /// <response code="400">Caso haja algum erro na validação dos dados.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Usuario>> Cadastrar([FromBody] Usuario usuario)
        {
            usuario = await _usuarioRepository.Adicionar(usuario);
            return CreatedAtAction(nameof(BuscarPorId), new { id = usuario.UsuarioId }, usuario);
        }

        /// <summary>
        /// Lista todos os usuários cadastrados.
        /// </summary>
        /// <returns>Lista de usuários.</returns>
        /// <response code="200">Caso a lista de usuários seja recuperada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Usuario>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Usuario>>> BuscarUsuarios()
        {
            List<Usuario> usuarios = await _usuarioRepository.BuscarUsuarios();
            return Ok(usuarios);
        }

        /// <summary>
        /// Busca um usuário pelo ID.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <returns>O usuário encontrado.</returns>
        /// <response code="200">Caso o usuário seja encontrado.</response>
        /// <response code="404">Caso o usuário não seja encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Usuario>> BuscarPorId(int id)
        {
            Usuario usuario = await _usuarioRepository.BuscarPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        /// <summary>
        /// Atualiza os dados de um usuário.
        /// </summary>
        /// <param name="usuario">Objeto contendo os dados do usuário.</param>
        /// <param name="id">Identificador do usuário.</param>
        /// <returns>O usuário atualizado.</returns>
        /// <response code="200">Caso o usuário seja atualizado com sucesso.</response>
        /// <response code="400">Caso haja algum erro na validação dos dados.</response>
        /// <response code="404">Caso o usuário não seja encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Usuario>> Atualizar([FromBody] Usuario usuario, int id)
        {
            usuario.UsuarioId = id;
            usuario = await _usuarioRepository.Atualizar(usuario, id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        /// <summary>
        /// Apaga um usuário pelo ID.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <returns>Indica se o usuário foi apagado com sucesso.</returns>
        /// <response code="200">Caso o usuário seja apagado com sucesso.</response>
        /// <response code="404">Caso o usuário não seja encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> Apagar(int id)
        {
            bool apagado = await _usuarioRepository.Apagar(id);
            if (!apagado)
            {
                return NotFound();
            }
            return Ok(apagado);
        }
    }
}
