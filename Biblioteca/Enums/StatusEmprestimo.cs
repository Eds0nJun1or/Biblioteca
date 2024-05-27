using System.ComponentModel;

namespace Biblioteca.Enums
{
    public enum StatusEmprestimo
    {
        [Description("EmAndamento")]
        EmAndamento = 0,
        [Description("Renovado")]
        Renovado = 1,
        [Description("Devolvido")]
        Devolvido = 2,
        [Description("Atrasado")]
        Atrasado = 3
    }
}
