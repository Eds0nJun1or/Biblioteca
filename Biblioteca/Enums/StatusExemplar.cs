using System.ComponentModel;

namespace Biblioteca.Enums
{
    public enum StatusExemplar
    {
        [Description("Disponivel")]
        Disponivel = 0,
        [Description("Emprestado")]
        Emprestado = 1,
        [Description("Danificado")]
        Danificado = 2,
        [Description("Perdido")]
        Perdido = 3
    }
}
