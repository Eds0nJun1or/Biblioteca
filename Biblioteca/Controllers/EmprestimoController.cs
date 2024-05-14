using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Enums;

namespace Biblioteca.Controllers
{
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
        public async Task<IActionResult> AdicionaEmprestimo([FromBody] CreateEmprestimoDto emprestimoDto)
        {
            var livro = await _context.Livros.FindAsync(emprestimoDto.ExemplarId);
            var usuario = await _context.Usuarios.FindAsync(emprestimoDto.UsuarioId);

            if (livro == null || usuario == null)
            {
                return NotFound("Livro ou usuário não encontrado.");
            }

            int livrosEmprestados = await _context.Emprestimos
                .Where(e => e.UsuarioId == usuario.Id && e.Status == StatusEmprestimo.Ativo)
                .CountAsync();

            if (livrosEmprestados >= 3)
            {
                return Conflict("O usuário já possui o limite máximo de 3 livros emprestados.");
            }

            var emprestimo = _mapper.Map<Emprestimo>(emprestimoDto);
            emprestimo.Livro = livro;
            emprestimo.Usuario = usuario;
            emprestimo.DatahoraEmprestimo = DateTime.UtcNow;

            await _context.AddAsync(emprestimo);
            await _context.SaveChangesAsync();

            var emprestimoDtoResponse = _mapper.Map<ReadEmprestimoDto>(emprestimo);
            return CreatedAtAction(nameof(RecuperaEmprestimoPorId), new { id = emprestimo.Id }, emprestimoDtoResponse);
        }

        /// <summary>
        /// Consulta os empréstimos feitos por um usuário.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Lista de empréstimos.</returns>
        /// <response code="200">Caso o usuário seja encontrado e a consulta seja bem-sucedida.</response>
        /// <response code="404">Caso o usuário não seja encontrado.</response>
        [HttpGet("LivrosEmprestados/{usuarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConsultaLivrosEmprestadosUsuario(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var loans = await _context.Emprestimos
                .Where(e => e.UsuarioId == usuarioId && e.Status == StatusEmprestimo.Ativo)
                .Select(e => new {
                    Id = e.Id,
                    DatahoraEmprestimo = e.DatahoraEmprestimo,
                    Exemplar = new { Id = e.ExemplarId }, 
                    Livro = new { Id = e.Exemplar.LivroId } 
                })
                .ToListAsync();

            if (loans.Count == 0)
            {
                return NotFound("Nenhum empréstimo encontrado para este usuário.");
            }

            return Ok(loans);
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
        public async Task<IActionResult> RecuperaEmprestimoPorId(int id)
        {
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                .Include(e => e.Exemplar.Livro)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprestimo == null)
            {
                return NotFound("Empréstimo não encontrado.");
            }

            var emprestimoDto = _mapper.Map<ReadEmprestimoDto>(emprestimo);

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
        public async Task<IActionResult> AtualizaEmprestimo(int id, [FromBody] UpdateEmprestimoDto emprestimoDto)
        {
            if (id != emprestimoDto.Id)
            {
                return BadRequest("O ID do empréstimo não corresponde ao ID fornecido.");
            }

            var emprestimo = await _context.Emprestimos
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprestimo == null)
            {
                return NotFound("Empréstimo não encontrado.");
            }

            if (emprestimo.UsuarioId != emprestimoDto.UsuarioId)
            {
                int livrosEmprestadosNovoUsuario = await _context.Emprestimos
                    .Where(e => e.UsuarioId == emprestimoDto.UsuarioId && e.Status == StatusEmprestimo.Ativo)
                    .CountAsync();

                if (livrosEmprestadosNovoUsuario >= 3)
                {
                    return BadRequest("O novo usuário já possui o limite máximo de 3 livros emprestados.");
                }
            }

            _mapper.Map(emprestimoDto, emprestimo);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

