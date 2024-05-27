using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Multa
    {
        [Key]
        [Required(ErrorMessage = "ID da multa é obrigatório.")]
        public int MultaId { get; set; }

        [Required(ErrorMessage = "O ID do empréstimo da multa é obrigatório.")]
        public int EmprestimoId { get; set; }

        [Required(ErrorMessage = "O valor da multa é obrigatório.")]
        public double Valor { get; set; }

        [Required(ErrorMessage = "A data de início da multa é obrigatória.")]
        public DateTime InicioMulta { get; set; }

        public DateTime? FimMulta { get; set; }

        [Required(ErrorMessage = "O número de dias atrasados da multa é obrigatório.")]
        public int DiasAtrasados { get; set; }

        [Required(ErrorMessage = "O status da multa é obrigatório.")]
        public StatusMulta Status { get; set; }

        public Emprestimo Emprestimo { get; set; }

        public Usuario Usuario { get; set; }
    }
}
