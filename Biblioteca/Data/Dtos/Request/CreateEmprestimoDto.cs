using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class CreateEmprestimoDto
    {
        [Required(ErrorMessage = "O ID do usu�rio do empr�stimo � obrigat�rio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O ID do exemplar do empr�stimo � obrigat�rio")]
        public int ExemplarId { get; set; }

        [Required(ErrorMessage = "A data e hora do empr�stimo � obrigat�ria")]
        public DateTime DatahoraEmprestimo { get; set; }

        [Required(ErrorMessage = "A data prevista para devolu��o � obrigat�ria")]
        public DateTime DataPrevistaDevolucao { get; set; }

        [Required(ErrorMessage = "O status do empr�stimo � obrigat�rio")]
        public StatusEmprestimo Status { get; set; }
    }
}