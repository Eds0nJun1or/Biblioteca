using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models;

public class livro
{
    [Key]
    [Required(ErrorMessage = "ID do livro é obrigatório")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O título do livro é obrigatório")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O autor do livro é obrigatório")]
    public string Autor { get; set; }

    [Required(ErrorMessage = "A editora do livro é obrigatória")]
    public string Editora { get; set; }

    [Required(ErrorMessage = "A data de publicação do livro é obrigatória")]
    public DateOnly DataPublicacao { get; set; }

    [Required(ErrorMessage = "O gênero do livro é obrigatório")]
    [MaxLength(50, ErrorMessage = "O tamanho do gênero não pode exceder 50 caracteres")]
    public string Genero { get; set; }

    [Required(ErrorMessage = "A quantidade de páginas do livro é obrigatória")]
    [Range(45, 1000, ErrorMessage = "O livro deve ter entre 45 e 1000 páginas")]
    public int Paginas { get; set; }

    [Required(ErrorMessage = "O valor do livro é obrigatório")]
    public float Valor { get; set; }

    [Required(ErrorMessage = "O status do livro é obrigatório")]
    public string Status { get; set; }
}