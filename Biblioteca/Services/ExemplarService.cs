using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Data;
using Biblioteca.Enums;
using Biblioteca.Models;

namespace Biblioteca.Services
{
    /// <summary>
    /// Serviço para gerenciar exemplares de livros.
    /// </summary>
    public class ExemplarService : IExemplarService
    {
        private readonly BibliotecaContext _context;

        /// <summary>
        /// Construtor do serviço de exemplares.
        /// </summary>
        /// <param name="context">Contexto do banco de dados da biblioteca.</param>
        public ExemplarService(BibliotecaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um novo exemplar com base nos dados fornecidos.
        /// </summary>
        /// <param name="exemplarDto">Objeto DTO contendo os dados do exemplar a ser criado.</param>
        /// <exception cref="ArgumentException">Lançado quando o livro não é encontrado ou o status do exemplar é inválido.</exception>
        public void CreateExemplar(CreateExemplarDto exemplarDto)
        {
            // Verifica se o livro existe
            var livro = _context.Livros.FirstOrDefault(l => l.LivroId == exemplarDto.LivroId);
            if (livro == null)
            {
                throw new ArgumentException("Livro não encontrado.");
            }

            // Verifica se o status é válido
            if (exemplarDto.Status == StatusExemplar.Emprestado || exemplarDto.Status == StatusExemplar.Danificado || exemplarDto.Status == StatusExemplar.Perdido)
            {
                throw new ArgumentException("Não é possível criar um exemplar com status emprestado, danificado ou perdido.");
            }

            // Cria o exemplar
            var exemplar = new Exemplar
            {
                LivroId = exemplarDto.LivroId,
                Status = exemplarDto.Status
            };

            _context.Exemplares.Add(exemplar);
            _context.SaveChanges();
        }

        /// <summary>
        /// Retorna todos os exemplares.
        /// </summary>
        /// <returns>Uma coleção de DTOs de leitura de exemplares.</returns>
        public IEnumerable<ReadExemplarDto> GetAllExemplares()
        {
            return _context.Exemplares
                .Select(e => new ReadExemplarDto
                {
                    ExemplarId = e.ExemplarId,
                    LivroId = e.LivroId,
                    Status = e.Status.ToString()
                })
                .ToList();
        }

        /// <summary>
        /// Retorna os exemplares associados a um ID de livro específico.
        /// </summary>
        /// <param name="livroId">ID do livro para o qual buscar os exemplares.</param>
        /// <returns>Uma coleção de DTOs de leitura de exemplares.</returns>
        public IEnumerable<ReadExemplarDto> GetExemplaresByLivroId(int livroId)
        {
            return _context.Exemplares
                .Where(e => e.LivroId == livroId)
                .Select(e => new ReadExemplarDto
                {
                    ExemplarId = e.ExemplarId,
                    LivroId = e.LivroId,
                    Status = e.Status.ToString()
                })
                .ToList();
        }
    }
}
