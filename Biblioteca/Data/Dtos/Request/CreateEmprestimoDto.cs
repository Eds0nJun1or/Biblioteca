using Biblioteca.Models;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class CreateEmprestimoDto
    {
        [Key]
        [Required(ErrorMessage = "ID do empr�stimo � obrigat�rio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do usu�rio do empr�stimo � obrigat�rio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O ID do exemplar do empr�stimo � obrigat�rio")]
        public int ExemplarId { get; set; }

        [Required(ErrorMessage = "A data e hora do empr�stimo � obrigat�ria")]
        public DateTime DatahoraEmprestimo { get; set; }

        [Required(ErrorMessage = "A data prevista de devolu��o do empr�stimo � obrigat�ria")]
        public DateOnly DataPrevistaInicial { get; set; }

        public DateOnly? DataDevolucao { get; set; }

        [Required(ErrorMessage = "O status do empr�stimo � obrigat�rio")]
        public int Status { get; set; }
        public int LivroId { get; internal set; }
    }
}