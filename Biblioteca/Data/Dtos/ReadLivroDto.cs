using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Data.Dtos
{
    public class ReadLivroDto
    {
        public string Titulo { get; set; }
        public string Genero { get; set; }
        public int Paginas { get; set; }
        public DateTime HoraDaConsulta { get; set; } = DateTime.Now;
    }
}
