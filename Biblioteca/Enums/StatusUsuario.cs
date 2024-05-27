using System.ComponentModel;

namespace Biblioteca.Enums
{
    public enum StatusUsuario
    {
        [Description("Ativo")]
        Ativo = 1,
        [Description("Inativo")]
        Inativo = 2,
        [Description("Bloqueado")]
        Bloqueado = 3
    }
}
