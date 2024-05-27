using Biblioteca.Enums;

namespace Biblioteca.Data.Dtos.Response
{
    public class ReadEmprestimoDto
    {
        public int EmprestimoId { get; set; }
        public int UsuarioId { get; set; }
        public int ExemplarId { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataPrevistaDevolucao { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public StatusEmprestimo Status { get; set; }
        public string NomeUsuario { get; set; }
        public string TituloExemplar { get; set; }
        public bool? Multa { get; set; }
    }
}
