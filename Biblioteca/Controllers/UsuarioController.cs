using Biblioteca.Infra.Repositories;
using Biblioteca.Models;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> Cadastrar([FromBody] Usuario Usuario)
        {
            Usuario usuario = await _usuarioRepository.Adicionar(Usuario);
            return Ok(usuario);
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> BuscarUsuarios()
        {
            List<Usuario> usuarios = await _usuarioRepository.BuscarUsuarios();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Usuario>>> BuscarPorId(int id)
        {
            Usuario usuario = await _usuarioRepository.BuscarPorId(id);
            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Usuario>> Atualizar ([FromBody] Usuario Usuario, int id)
        {
            Usuario.Id = id;
            Usuario usuario = await _usuarioRepository.Atualizar(Usuario, id);
            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Usuario>> Apagar(int id)
        {
            bool apagado = await _usuarioRepository.Apagar(id);
            return Ok(apagado);
        }
    }
}
