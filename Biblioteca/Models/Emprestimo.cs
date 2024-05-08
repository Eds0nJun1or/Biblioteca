using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Emprestimo
    {
        [Key]
        [Required(ErrorMessage = "ID do empréstimo é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do usuário do empréstimo é obrigatório")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O ID do exemplar do empréstimo é obrigatório")]
        public int ExemplarId { get; set; }

        [Required(ErrorMessage = "A data e hora do empréstimo é obrigatória")]
        public DateTime DatahoraEmprestimo { get; set; }

        [Required(ErrorMessage = "A data prevista de devolução do empréstimo é obrigatória")]
        public DateOnly DataPrevistaInicial { get; set; }

        public DateOnly? DataDevolucao { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório")]
        public StatusEmprestimo Status { get; set; }

        public Usuario Usuario { get; set; }
        public Exemplar Exemplar { get; set; }
    }
}
