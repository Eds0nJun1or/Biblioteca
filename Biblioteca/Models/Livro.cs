using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models;

public class Livro
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required(ErrorMessage = "O título do livro é obrigatório")]
    public string Titulo { get; set; }
    [Required(ErrorMessage = "O gênero do livro é obrigatório")]
    [MaxLength(50, ErrorMessage = "O tamanho do gênero não pode exceder 30 caracteres")]
    public string Genero { get; set; }
    [Required]
    [Range(45, 1000, ErrorMessage = "O livro deve ter entre 45 e 1000 páginas")]
    public int Paginas { get; set; }
}

