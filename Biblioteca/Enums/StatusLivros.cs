using System.ComponentModel;
using System.Reflection;

namespace Biblioteca.Enums
{
    public enum StatusLivros
    {
        [Description("Disponivel")]
        Disponivel = 0,
        [Description("Emprestado")]
        Emprestado = 1,
        [Description("Reservado")]
        Reservado = 2
    }
}

