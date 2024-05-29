using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Request
{
    public class CreateMultaDto
    {
        [Required(ErrorMessage = "O ID do empréstimo da multa é obrigatório")]
        public int EmprestimoId { get; set; }

        [Required(ErrorMessage = "O número de dias atrasados da multa é obrigatório")]
        public int DiasAtrasados { get; set; }
    }
}
