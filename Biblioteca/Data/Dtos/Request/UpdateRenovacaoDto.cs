using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class UpdateRenovacaoDto
    {
        [Required(ErrorMessage = "ID do empréstimo é obrigatório")]
        public int EmprestimoId { get; set; }

        [Required(ErrorMessage = "A data para devolução é obrigatória")]

        public DateTime DataDevolucao { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório")]
        public StatusEmprestimo Status { get; set; }
    }
}
