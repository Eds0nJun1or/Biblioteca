﻿using Biblioteca.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Biblioteca.Models
{
    public class Emprestimo
    {
        [Key]
        [Required(ErrorMessage = "ID do empréstimo é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do usuário do empréstimo é obrigatório")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O ID do exemplar do empréstimo é obrigatório")]
        public int ExemplarId { get; set; }

        [Required(ErrorMessage = "A data e hora do empréstimo é obrigatória")]
        public DateTime DatahoraEmprestimo { get; set; }

        public DateTime DataPrevistaDevolucao { get; set; }

        public DateOnly? DataDevolucao { get; set; }

        [Required(ErrorMessage = "O status do empréstimo é obrigatório")]
        public StatusEmprestimo Status { get; set; }

        [JsonIgnore]
        public Usuario Usuario { get; set; }
        [JsonIgnore]
        public Exemplar Exemplar { get; set; }
        [JsonIgnore]
        public Livro Livro { get; internal set; }
    }
}

