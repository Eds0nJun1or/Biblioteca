using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class CreateEmprestimoDto
    {
        [Required(ErrorMessage = "O ID do usuário do empréstimo é obrigatório")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O ID do exemplar do empréstimo é obrigatório")]
        public int ExemplarId { get; set; }

        [Required(ErrorMessage = "A data do empréstimo é obrigatório")]
        public DateTime DataEmprestimo { get; set; }
    }
}