using Biblioteca.Enums;
using Biblioteca.Models;

namespace Biblioteca.Data.Dtos.Response
{
    public class ReadMultaDto
    {
        public int Id { get; set; }
        public int EmprestimoId { get; set; }
        public double Valor { get; set; }
        public DateTime InicioMulta { get; set; }
        public DateTime? FimMulta { get; set; }
        public int DiasAtrasados { get; set; }
        public StatusMulta Status { get; set; }
        public Emprestimo Emprestimo { get; set; }
        public string NomeUsuario { get; set; }
        public string TituloLivro { get; set; }
        public int UsuarioId { get; set; }
    }
} 
