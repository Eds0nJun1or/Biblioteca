using Biblioteca.Enums;

namespace Biblioteca.Data.Dtos.Response
{
    public class ReadEmprestimoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ExemplarId { get; set; }
        public DateTime DatahoraEmprestimo { get; set; }
        public DateTime DataPrevistaDevolucao { get; set; }
        public DateOnly? DataDevolucao { get; set; }
        public StatusEmprestimo Status { get; set; }
        public string NomeUsuario { get; set; }
        public string TituloExemplar { get; set; }
        public bool UsuarioAtingiuLimiteEmprestimos { get; internal set; }
    }
}
