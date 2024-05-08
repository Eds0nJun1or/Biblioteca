using System.ComponentModel;

namespace Biblioteca.Enums
{
    public enum StatusMulta
    {
        [Description("Pendente")]
        Pendente = 0,
        [Description("Pago")]
        Pago = 1,
        [Description("Cancelado")]
        Cancelado = 2
    }
}
