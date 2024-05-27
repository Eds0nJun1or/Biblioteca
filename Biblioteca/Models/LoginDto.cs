using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O campo 'CPF' é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O campo 'CPF' deve conter 11 dígitos.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O campo 'Senha' é obrigatório.")]
        [MinLength(6)]
        public string Senha { get; set; }
    }
}
