using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Biblioteca.Models;

namespace Biblioteca.Controllers;

[ApiController]
[Route("[controller]")]
public class LivroController : ControllerBase
{

    private static List<Livro> livros = new List<Livro>();
    private static int id = 0;

    [HttpPost]
    public IActionResult AdicionaLivro([FromBody] Livro livro)
    {
        livro.Id = id++;
        livros.Add(livro);
        return CreatedAtAction(nameof(RecuperaLivroPorId),
            new { id = livro.Id },
            livro);
    }

    [HttpGet]
    public IEnumerable<Livro> RecuperaLivros([FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        return livros.Skip(skip).Take(take);
    }

    [HttpGet("{id}")]
    public IActionResult RecuperaLivroPorId(int id)
    {
        var livro = livros.FirstOrDefault(livro => livro.Id == id);
        if (livro == null) return NotFound();
        return Ok(livro);
    }
}

