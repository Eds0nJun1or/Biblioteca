using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class CreateExemplarDto
    {
        [Required(ErrorMessage = "O ID do livro é obrigatório")]
        public int LivroId { get; set; }

        [Required(ErrorMessage = "O status do exemplar é obrigatório")]
        public StatusExemplar Status { get; set; }
    }
}
