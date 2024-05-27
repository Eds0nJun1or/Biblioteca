using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class CreateEmprestimoDto
    {
        [Required(ErrorMessage = "O ID do usu�rio do empr�stimo � obrigat�rio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O ID do exemplar do empr�stimo � obrigat�rio")]
        public int ExemplarId { get; set; }

        [Required(ErrorMessage = "A data do empr�stimo � obrigat�rio")]
        public DateTime DataEmprestimo { get; set; }
    }
}