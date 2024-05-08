using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Exemplar
    {
        [Key]
        [Required(ErrorMessage = "ID do exemplar é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do livro do exemplar é obrigatório")]
        public int LivroId { get; set; }

        [Required(ErrorMessage = "O status do exemplar é obrigatório")]
        public StatusExemplar Status { get; set; }

        public Livro Livro { get; set; }

        public virtual ICollection<Emprestimo> Emprestimos { get; set; }
    }
}
