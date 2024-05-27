using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos.Reponse
{
    public class ReadLivroDto
    {
        public int LivroId { get; set; }
        public string Nome { get; set; }
        public string Autor { get; set; }
        public string Editora { get; set; }
        public DateOnly DataPublicacao { get; set; }
        public string Genero { get; set; }
        public int Paginas { get; set; }
        public float Valor { get; set; }
        public int Status { get; set; }
        public DateTime HoraDaConsulta { get; set; } = DateTime.Now;
    }
}