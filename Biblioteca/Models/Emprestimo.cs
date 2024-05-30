using Biblioteca.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public class Emprestimo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmprestimoId { get; set; }

        [Required(ErrorMessage = "O ID do usuário do empréstimo é obrigatório.")]
       public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O ID do exemplar do empréstimo é obrigatório.")]
        public int ExemplarId { get; set; }

        [Required]
        public int FuncionarioId { get; set; }

        [Required(ErrorMessage = "A data e hora do empréstimo é obrigatória.")]
        public DateTime DataEmprestimo { get; set; }

        [Required]
        public DateTime DataPrevistaDevolucao { get; set; }
        public DateTime? DataDevolucao { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório.")]
        public StatusEmprestimo Status { get; set; }

        [JsonIgnore]
        public Usuario Usuario { get; set; }

        [JsonIgnore]
        public Exemplar Exemplar { get; set; }

        [JsonIgnore]
        public int LivroId { get; set; }

        public virtual ICollection<Multa> Multas { get; set; }

    }
}

