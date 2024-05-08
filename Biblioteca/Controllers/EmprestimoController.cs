using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Enums;
//using Microsoft.AspNetCore.Authorization;


namespace Biblioteca.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EmprestimoController : ControllerBase
    {
        private readonly BibliotecaContext _context;
        private readonly IMapper _mapper;

        public EmprestimoController(BibliotecaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Adiciona um novo empréstimo de livro.
        /// </summary>
        /// <param name="emprestimoDto">Objeto contendo os dados do empréstimo.</param>
        /// <returns>IActionResult</returns>
        /// <response code="201">Caso o empréstimo seja criado com sucesso.</response>
        /// <response code="400">Caso haja algum erro na validação dos dados.</response>
        /// <response code="404">Caso o livro ou o usuário não sejam encontrados.</response>
        /// <response code="409">Caso o usuário já tenha o limite máximo de empréstimos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult AdicionaEmprestimo([FromBody] CreateEmprestimoDto emprestimoDto)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.Id == emprestimoDto.LivroId);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == emprestimoDto.UsuarioId);

            int livrosEmprestados = _context.Emprestimos
                .Where(e => e.UsuarioId == usuario.Id && e.Status == StatusEmprestimo.Ativo)
                .Count();

            if (livrosEmprestados >= 3)
            {
                return BadRequest("O usuário já possui o limite máximo de 3 livros emprestados.");
            }

            Emprestimo emprestimo = _mapper.Map<Emprestimo>(emprestimoDto);

            _context.Emprestimos.Add(emprestimo);
            _context.SaveChanges();

            var emprestimoDtoResponse = _mapper.Map<ReadEmprestimoDto>(emprestimo);
            return CreatedAtAction(nameof(RecuperaEmprestimoPorId), new { id = emprestimo.Id }, emprestimoDtoResponse);
        }

        /// <summary>
        /// Consulta a quantidade de livros emprestados por um usuário.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Quantidade de livros emprestados.</returns>
        /// <response code="200">Caso o usuário seja encontrado e a consulta seja bem-sucedida.</response>
        /// <response code="404">Caso o usuário não seja encontrado.</response>
        [HttpGet("LivrosEmprestados/{usuarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ConsultaLivrosEmprestadosUsuario(int usuarioId)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            int livrosEmprestados = _context.Emprestimos
                .Where(e => e.UsuarioId == usuario.Id && e.Status == StatusEmprestimo.Ativo)
                .Count();

            return Ok(livrosEmprestados);
        }

        /// <summary>
        /// Recupera um empréstimo específico por ID.
        /// </summary>
        /// <param name="id">ID do empréstimo.</param>
        /// <returns>Detalhes do empréstimo.</returns>
        /// <response code="200">Caso o empréstimo seja encontrado.</response>
        /// <response code="404">Caso o empréstimo não seja encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RecuperaEmprestimoPorId(int id)
        {
            var emprestimo = _context.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                .Include(e => e.Exemplar.Livro)
                .FirstOrDefault(e => e.Id == id);

            if (emprestimo == null)
            {
                return NotFound("Empréstimo não encontrado.");
            }

            var usuarioPossuiLimite = _context.Emprestimos
                .Where(e => e.UsuarioId == emprestimo.UsuarioId && e.Status == StatusEmprestimo.Ativo)
                .Count() >= 3;

            var emprestimoDto = _mapper.Map<ReadEmprestimoDto>(emprestimo);
            emprestimoDto.UsuarioAtingiuLimiteEmprestimos = usuarioPossuiLimite;

            return Ok(emprestimoDto);
        }

        /// <summary>
        /// Atualiza um empréstimo por ID.
        /// </summary>
        /// <param name="id">ID do empréstimo.</param>
        /// <param name="emprestimoDto">Objeto contendo os dados atualizados do empréstimo.</param>
        /// <returns>NoContent caso a atualização seja bem-sucedida, NotFound caso o empréstimo não seja encontrado.</returns>
        /// <response code="204">Caso a atualização seja realizada com sucesso.</response>
        /// <response code="404">Caso o empréstimo não seja encontrado.</response>
        /// <response code="409">Caso o usuário já tenha o limite máximo de empréstimos.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult AtualizaEmprestimo(int id, [FromBody] UptadeEmprestimoDto emprestimoDto)
        {
            if (id != emprestimoDto.Id)
            {
                return BadRequest("O ID do empréstimo não corresponde ao ID fornecido.");
            }

            var emprestimo = _context.Emprestimos
                .Include(e => e.Usuario)
                .FirstOrDefault(e => e.Id == id);

            if (emprestimo == null)
            {
                return NotFound("Empréstimo não encontrado.");
            }

            if (emprestimo.UsuarioId != emprestimoDto.UsuarioId)
            {
                int livrosEmprestadosNovoUsuario = _context.Emprestimos
                    .Where(e => e.UsuarioId == emprestimoDto.UsuarioId && e.Status == StatusEmprestimo.Ativo)
                    .Count();

                if (livrosEmprestadosNovoUsuario >= 3)
                {
                    return BadRequest("O novo usuário já possui o limite máximo de 3 livros emprestados.");
                }
            }

            _mapper.Map(emprestimoDto, emprestimo);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
