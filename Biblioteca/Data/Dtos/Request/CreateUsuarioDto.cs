using Biblioteca.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Biblioteca.Data.Dtos.Request
{
    public class CreateUsuarioDto
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        [StringLength(11)]
        public string Cpf { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(11)]
        public string Telefone { get; set; }

        [JsonIgnore]
        public List<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();
    }
}
