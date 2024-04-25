using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Biblioteca.Models;
using Biblioteca.Data;
using Biblioteca.Data.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace Biblioteca.Controllers;

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
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaLivro(
        [FromBody] CreateLivroDto livroDto)
    {
        livro livro = _mapper.Map<livro>(livroDto);
        _context.Livros.Add(livro);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperaLivroPorId),
            new { id = livro.Id },
            livro);
    }

    [HttpGet]
    public IEnumerable<ReadLivroDto> RecuperaLivros([FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        return _mapper.Map<List<ReadLivroDto>>(_context.Livros.Skip
            (skip).Take(take));
    }

    [HttpGet("{id}")]
    public IActionResult RecuperaLivroPorId(int id)
    {
        var livro = _context.Livros
            .FirstOrDefault(livro => livro.Id == id);
        if (livro == null) return NotFound();
        var livroDto = _mapper.Map<ReadLivroDto>(livro);
        return Ok(livroDto);
    }

    [HttpPut("{id}")]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateLivroDto livroDto)
    {
        var livro = _context.Livros.FirstOrDefault(l => l.Id == id);
        if (livro == null)
            return NotFound();
        _mapper.Map(livroDto, livro);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult AtualizaLivroParcial(int id, 
        JsonPatchDocument<UpdateLivroDto> patch)
    {
        var livro = _context.Livros.FirstOrDefault(
            livro => livro.Id == id);
        if (livro == null) return NotFound();

        var livroParaAtualizar = _mapper.Map<UpdateLivroDto>(livro);

        patch.ApplyTo(livroParaAtualizar, ModelState);

        if(!TryValidateModel(livroParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }
        _mapper.Map(livroParaAtualizar, livro);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletaFilme(int id)
    {
        var livro = _context.Livros.FirstOrDefault(
            livro => livro.Id == id);
        if (livro == null) return NotFound();
        _context.Remove(livro);
        _context.SaveChanges();
        return NoContent();
    }
}