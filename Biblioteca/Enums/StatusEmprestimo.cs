using System.ComponentModel;

namespace Biblioteca.Enums
{
    public enum StatusEmprestimo
    {
        [Description("Ativo")]
        Ativo = 0,
        [Description("Encerrado")]
        Encerrado = 1,
        [Description("Atrasado")]
        Atrasado = 2
    }
}
