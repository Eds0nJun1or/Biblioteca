namespace Biblioteca.Data.Dtos.Response
{
    public class ReadFuncionarioDto
    {
        public int FuncionarioId { get; set; }
        public string Cpf { get; set; }
        public string NomeFuncionario { get; set; }
        public DateOnly DataNascimento { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
    }
}

