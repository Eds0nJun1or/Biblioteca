namespace Biblioteca.Data.Dtos.Response
{
    public class ReadEmprestimoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ExemplarId { get; set; }
        public DateTime DatahoraEmprestimo { get; set; }
        public DateOnly DataPrevistaInicial { get; set; }
        public DateOnly? DataDevolucao { get; set; }
        public int Status { get; set; }
        public DateTime HoraDaConsulta { get; set; } = DateTime.Now;

        public bool UsuarioAtingiuLimiteEmprestimos { get; set; }
    }
}
