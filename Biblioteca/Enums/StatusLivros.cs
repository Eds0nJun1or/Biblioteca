﻿using System.ComponentModel;

namespace Biblioteca.Enums
{
    public enum StatusLivros
    {
        [Description("Disponivel")]
        Disponivel = 0,
        [Description("Emprestado")]
        Emprestado = 1
    }
}

