using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class UpdateEmprestimoDto
    {
        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "ID do empréstimo é obrigatório")]
        public int EmprestimoId { get; set; }

        [Required(ErrorMessage = "ID do exemplar é obrigatório")]
        public int ExemplarId { get; set; }

        [Required(ErrorMessage = "A data e hora do empréstimo é obrigatória")]
        public DateTime DataEmprestimo { get; set; }

        [Required(ErrorMessage = "A data prevista para devolução é obrigatória")]
        public DateTime DataPrevistaDevolucao { get; set; }

        public DateTime? DataDevolucao { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório")]
        public StatusEmprestimo Status { get; set; }    }
}
