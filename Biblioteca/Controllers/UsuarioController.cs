using Biblioteca.Models;
using Biblioteca.Services;

//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    //[Authorize]
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
        public async Task<ActionResult<Usuario>> Cadastrar([FromBody] Usuario Usuario)
        {
            Usuario usuario = await _usuarioRepository.Adicionar(Usuario);
            return Ok(usuario);
        }

        /// <summary>
        /// Lista todos os usuários cadastrados.
        /// </summary>
        /// <returns>Lista de usuários.</returns>
        /// <response code="200">Caso a lista de usuários seja recuperada com sucesso.</response>
        [HttpGet]
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
        public async Task<ActionResult<List<Usuario>>> BuscarPorId(int id)
        {
            Usuario usuario = await _usuarioRepository.BuscarPorId(id);
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
        public async Task<ActionResult<Usuario>> Atualizar ([FromBody] Usuario Usuario, int id)
        {
            Usuario.Id = id;
            Usuario usuario = await _usuarioRepository.Atualizar(Usuario, id);
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
        public async Task<ActionResult<Usuario>> Apagar(int id)
        {
            bool apagado = await _usuarioRepository.Apagar(id);
            return Ok(apagado);
        }
    }
}
