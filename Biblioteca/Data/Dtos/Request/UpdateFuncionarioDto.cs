using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class UpdateFuncionarioDto
    {
        [Required(ErrorMessage = "ID do Funcionário é obrigatório")]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "O campo 'Senha' é obrigatório.")]
        [MinLength(6)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O campo 'E-mail' é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo 'Telefone' é obrigatório.")]
        [StringLength(11)]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O status do funcionário é obrigatório")]
        public StatusFuncionario Status { get; set; }
    }
}
