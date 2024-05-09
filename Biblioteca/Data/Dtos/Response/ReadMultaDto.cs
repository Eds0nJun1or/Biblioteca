﻿using Biblioteca.Enums;
using Biblioteca.Models;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Response
{
    public class ReadMultaDto
    {
        public int Id { get; set; }

        public int EmprestimoId { get; set; }

        public float Valor { get; set; }

        public DateTime InicioMulta { get; set; }

        public DateTime? FimMulta { get; set; }

        public int DiasAtrasados { get; set; }

        public StatusMulta Status { get; set; }

        public Emprestimo Emprestimo { get; set; }
    }
}