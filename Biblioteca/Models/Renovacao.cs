using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Renovacao
    {
        [Key]
        [Required(ErrorMessage = "ID da renovação é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do empréstimo da renovação é obrigatório")]
        public int EmprestimoId { get; set; }

        [Required(ErrorMessage = "A data da renovação é obrigatória")]
        public DateTime DataRenovacao { get; set; }

        public DateTime? NovaDataPrev { get; set; }

        public Emprestimo Emprestimo { get; set; }
    }
}

