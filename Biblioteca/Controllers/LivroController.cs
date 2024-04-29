using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Biblioteca.Models;
using Biblioteca.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Reponse;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LivroController : ControllerBase
    {
        private LivroContext _context;
        private IMapper _mapper;

        public LivroController(LivroContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Adiciona um livro ao banco de dados
        /// </summary>
        /// <param name="livroDto">Objeto com os campos necessários para criação de um livro</param>
        /// <returns>IActionResult</returns>
        /// <response code="201">Caso inserção seja feita com sucesso</response>
        /// <response code="404">Caso o livro não seja encontrado</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AdicionaLivro([FromBody] CreateLivroDto livroDto)
        {
            livro livro = _mapper.Map<livro>(livroDto);
            _context.Livros.Add(livro);
            _context.SaveChanges();
            return Created(string.Empty, _mapper.Map<ReadLivroDto>(livro)); ;
        }

        /// <summary>
        /// Recupera a lista de livros
        /// </summary>
        /// <param name="skip">Número de registros a serem ignorados</param>
        /// <param name="take">Número máximo de registros a serem retornados</param>
        /// <returns>Lista de livros</returns>
        /// <response code="201">Caso inserção seja feita com sucesso</response>
        /// <response code="404">Caso o livro não seja encontrado</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IEnumerable<ReadLivroDto> RecuperaLivros([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            return _mapper.Map<List<ReadLivroDto>>(_context.Livros.Skip(skip).Take(take));
        }
        /// <summary>
        /// Recupera um livro por ID
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Detalhes do livro</returns>
        /// <response code="201">Caso inserção seja feita com sucesso</response>
        /// <response code="404">Caso o livro não seja encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RecuperaLivroPorId(int id)
        {
            var livro = _context.Livros.FirstOrDefault(livro => livro.Id == id);
            if (livro == null) return NotFound();
            var livroDto = _mapper.Map<ReadLivroDto>(livro);
            return Ok(livroDto);
        }
        /// <summary>
        /// Atualiza um livro por ID
        /// </summary>
        /// <param name="id">ID do livro a ser atualizado</param>
        /// <param name="livroDto">Objeto com os novos dados do livro</param>
        /// <returns>NoContent caso a atualização seja bem-sucedida, NotFound caso o livro não seja encontrado</returns>
        /// <response code="204">Caso a atualização seja realizada com sucesso</response>
        /// <response code="404">Caso o livro não seja encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AtualizaFilme(int id, [FromBody] UpdateLivroDto livroDto)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.Id == id);
            if (livro == null)
                return NotFound();
            _mapper.Map(livroDto, livro);
            _context.SaveChanges();
            return NoContent();
        }
        /// <summary>
        /// Atualiza parcialmente um livro por ID
        /// </summary>
        /// <param name="id">ID do livro a ser atualizado</param>
        /// <param name="patch">JsonPatchDocument com as modificações a serem aplicadas</param>
        /// <returns>NoContent caso a atualização seja bem-sucedida, NotFound caso o livro não seja encontrado</returns>
        /// <response code="204">Caso a atualização parcial seja realizada com sucesso</response>
        /// <response code="404">Caso o livro não seja encontrado</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AtualizaLivroParcial(int id, JsonPatchDocument<UpdateLivroDto> patch)
        {
            var livro = _context.Livros.FirstOrDefault(livro => livro.Id == id);
            if (livro == null) return NotFound();

            var livroParaAtualizar = _mapper.Map<UpdateLivroDto>(livro);

            patch.ApplyTo(livroParaAtualizar, ModelState);

            if (!TryValidateModel(livroParaAtualizar))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(livroParaAtualizar, livro);
            _context.SaveChanges();
            return NoContent();
        }
        /// <summary>
        /// Remove um livro do banco de dados pelo seu ID
        /// </summary>
        /// <param name="id">ID do livro a ser removido</param>
        /// <returns>NoContent caso a remoção seja bem-sucedida, NotFound caso o livro não seja encontrado</returns>
        /// <response code="204">Caso a remoção seja realizada com sucesso</response>
        /// <response code="404">Caso o livro não seja encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletaFilme(int id)
        {
            var livro = _context.Livros.FirstOrDefault(livro => livro.Id == id);
            if (livro == null) return NotFound();
            _context.Remove(livro);
            _context.SaveChanges();
            return NoContent();
        }
    }
}