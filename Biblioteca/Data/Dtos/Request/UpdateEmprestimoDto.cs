using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class UpdateEmprestimoDto
    {
        internal readonly int UsuarioId;

        [Required(ErrorMessage = "ID do empréstimo é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "A data e hora do empréstimo é obrigatória")]
        public DateTime DatahoraEmprestimo { get; set; }

        [Required(ErrorMessage = "A data prevista para devolução é obrigatória")]
        public DateTime DataPrevistaDevolucao { get; set; }

        public DateOnly? DataDevolucao { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório")]
        public StatusEmprestimo Status { get; set; }
    }
}
