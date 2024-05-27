using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Funcionario
    {
        [Key]
        [Required]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "O campo 'CPF' é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O campo 'CPF' deve conter 11 dígitos.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O campo 'Senha' é obrigatório.")]
        [MinLength(6)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O campo 'Nome de Funcionário' é obrigatório.")]
        public string NomeFuncionario { get; set; }

        [Required(ErrorMessage = "O campo 'Data de Nascimento' é obrigatório.")]
        [DataType(DataType.Date)]
        public DateOnly DataNascimento { get; set; }

        [Required(ErrorMessage = "O campo 'E-mail' é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo 'Telefone' é obrigatório.")]
        [StringLength(11)]
        public string Telefone { get; set; }

        [Required]
        public StatusFuncionario Status { get; set; }
    }
}
