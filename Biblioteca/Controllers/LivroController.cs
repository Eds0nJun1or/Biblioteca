using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Interfaces;
using AutoMapper;
using Biblioteca.Data.Dtos.Reponse;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteca.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LivroController : ControllerBase
    {
        private readonly ILivroService _livroService;
        private readonly IMapper _mapper;

        public LivroController(ILivroService livroService, IMapper mapper)
        {
            _livroService = livroService;
            _mapper = mapper;
        }

        /// <summary>
        /// Adiciona um novo livro.
        /// </summary>
        /// <param name="livroDto">Objeto DTO contendo os dados do livro a ser adicionado.</param>
        /// <returns>Retorna uma resposta HTTP 201 (Created) com o objeto do livro adicionado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdicionaLivro([FromBody] CreateLivroDto livroDto)
        {
            Livro livro = _mapper.Map<Livro>(livroDto);
            await _livroService.Adicionar(livro);
            return CreatedAtAction(nameof(RecuperaLivroPorId), new { id = livro.LivroId }, livro);
        }

        /// <summary>
        /// Recupera uma lista de livros.
        /// </summary>
        /// <param name="skip">Número de registros a serem ignorados.</param>
        /// <param name="take">Número de registros a serem retornados.</param>
        /// <returns>Retorna uma lista de objetos DTO de livros.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IEnumerable<ReadLivroDto>> RecuperaLivros([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            var livros = await _livroService.BuscarLivros();
            return _mapper.Map<List<ReadLivroDto>>(livros.Skip(skip).Take(take));
        }

        /// <summary>
        /// Recupera um livro pelo ID.
        /// </summary>
        /// <param name="id">ID do livro.</param>
        /// <returns>Retorna uma resposta HTTP 200 (OK) com o objeto DTO do livro encontrado, ou uma resposta HTTP 404 (Not Found) se o livro não for encontrado.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RecuperaLivroPorId(int id)
        {
            var livro = await _livroService.BuscarPorId(id);
            if (livro == null) return NotFound();
            var livroDto = _mapper.Map<ReadLivroDto>(livro);
            return Ok(livroDto);
        }

        /// <summary>
        /// Pesquisa livros com base nos parâmetros fornecidos.
        /// </summary>
        /// <param name="titulo">Título do livro (obrigatório).</param>
        /// <param name="autor">Autor do livro (opcional).</param>
        /// <param name="categoria">Categoria do livro (opcional).</param>
        /// <returns>Retorna uma resposta HTTP 200 (OK) com a lista de livros encontrados, ou uma resposta HTTP 404 (Not Found) se nenhum livro for encontrado.</returns>
        [HttpGet("Pesquisa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PesquisaLivros([FromQuery] string titulo, [FromQuery] string autor = null, [FromQuery] string categoria = null)
        {
            if (string.IsNullOrEmpty(titulo))
            {
                return BadRequest("O título é obrigatório.");
            }

            var livros = await _livroService.PesquisarLivros(titulo, autor, categoria);

            if (livros.Count == 0)
            {
                return NotFound("Nenhum livro encontrado com os parâmetros informados.");
            }

            return Ok(livros);
        }

        /// <summary>
        /// Atualiza um livro existente.
        /// </summary>
        /// <param name="id">ID do livro a ser atualizado.</param>
        /// <param name="livroDto">Objeto DTO contendo os novos dados do livro.</param>
        /// <returns>Retorna uma resposta HTTP 204 (No Content) se o livro for atualizado com sucesso, ou uma resposta HTTP 404 (Not Found) se o livro não for encontrado.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizaLivro(int id, [FromBody] UpdateLivroDto livroDto)
        {
            try
            {
                var livro = _mapper.Map<Livro>(livroDto);
                await _livroService.Atualizar(livro, id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deleta um livro pelo ID.
        /// </summary>
        /// <param name="id">ID do livro a ser deletado.</param>
        /// <returns>Retorna uma resposta HTTP 204 (No Content) se o livro for deletado com sucesso, ou uma resposta HTTP 404 (Not Found) se o livro não for encontrado.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletaLivro(int id)
        {
            try
            {
                await _livroService.Apagar(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
