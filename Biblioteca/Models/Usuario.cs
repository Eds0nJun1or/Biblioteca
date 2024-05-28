using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Biblioteca.Models
{
    public class Usuario
    {
        [Key]
        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O nome do usuário é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF do usuário é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O email do usuário é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        public StatusUsuario Status { get; set; } = StatusUsuario.Ativo;

        [JsonIgnore]
        public bool Bloqueado { get; set; }

        [JsonIgnore]
        public virtual ICollection<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
    }
}
