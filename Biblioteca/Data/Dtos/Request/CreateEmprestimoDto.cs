using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class CreateEmprestimoDto
    {
        [Required(ErrorMessage = "O ID do usuário do empréstimo é obrigatório")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O ID do exemplar do empréstimo é obrigatório")]
        public int ExemplarId { get; set; }

        [Required(ErrorMessage = "A data e hora do empréstimo é obrigatória")]
        public DateTime DatahoraEmprestimo { get; set; }

        [Required(ErrorMessage = "A data prevista para devolução é obrigatória")]
        public DateTime DataPrevistaDevolucao { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório")]
        public StatusEmprestimo Status { get; set; }
    }
}