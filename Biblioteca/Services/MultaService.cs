using AutoMapper;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Data;
using Biblioteca.Enums;
using Biblioteca.Exceptions;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Services
{
    public class MultaService : IMultaService
    {
        private readonly BibliotecaContext _context;
        private readonly IMapper _mapper;

        public MultaService(BibliotecaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> AdicionarMulta(CreateMultaDto multaDto)
        {
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                    .ThenInclude(ex => ex.Livro)
                .FirstOrDefaultAsync(e => e.EmprestimoId == multaDto.EmprestimoId);

            if (emprestimo == null)
            {
                throw new NotFoundException("Empréstimo não encontrado.");
            }

            var multa = _mapper.Map<Multa>(multaDto);
            multa.Emprestimo = emprestimo;
            multa.Usuario = emprestimo.Usuario;
            multa.InicioMulta = DateTime.UtcNow;
            multa.Status = StatusMulta.Pendente;

            await _context.Multas.AddAsync(multa);
            await _context.SaveChangesAsync();

            return multa.MultaId;
        }

        public async Task<ReadMultaDto> RecuperarMultaPorId(int multaId)
        {
            var multa = await _context.Multas
                .Include(m => m.Emprestimo)
                    .ThenInclude(e => e.Usuario)
                .Include(m => m.Emprestimo)
                    .ThenInclude(e => e.Exemplar)
                        .ThenInclude(ex => ex.Livro)
                .FirstOrDefaultAsync(m => m.MultaId == multaId);

            if (multa == null)
            {
                throw new NotFoundException("Multa não encontrada.");
            }

            var multaDto = _mapper.Map<ReadMultaDto>(multa);
            multaDto.NomeUsuario = multa.Emprestimo.Usuario.Nome;
            multaDto.TituloLivro = multa.Emprestimo.Exemplar.Livro.Nome;

            return multaDto;
        }

        public async Task AtualizarMulta(int multaId, UpdateMultaDto multaDto)
        {
            var multa = await _context.Multas.FirstOrDefaultAsync(m => m.MultaId == multaId);

            if (multa == null)
            {
                throw new NotFoundException("Multa não encontrada.");
            }

            _mapper.Map(multaDto, multa);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReadMultaDto>> RecuperarTodasAsMultas()
        {
            var multas = await _context.Multas
                .Include(m => m.Emprestimo)
                    .ThenInclude(e => e.Usuario)
                .Include(m => m.Emprestimo)
                    .ThenInclude(e => e.Exemplar)
                        .ThenInclude(ex => ex.Livro)
                .ToListAsync();

            var multasDto = _mapper.Map<List<ReadMultaDto>>(multas);
            foreach (var multa in multasDto)
            {
                var usuario = await _context.Usuarios.FindAsync(multa.UsuarioId);
                var exemplar = await _context.Exemplares.FindAsync(multa.Emprestimo.ExemplarId);
                var livro = await _context.Livros.FindAsync(exemplar.LivroId);
                multa.NomeUsuario = usuario.Nome;
                multa.TituloLivro = livro.Nome;
            }

            return multasDto;
        }

        public async Task PagarMulta(int multaId)
        {
            var multa = await _context.Multas
                .Include(m => m.Emprestimo)
                    .ThenInclude(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.MultaId == multaId);

            if (multa == null)
            {
                throw new NotFoundException("Multa não encontrada.");
            }

            if (multa.Status == StatusMulta.Paga)
            {
                throw new BadRequestException("A multa já foi paga.");
            }

            multa.Status = StatusMulta.Paga;
            multa.Emprestimo.Usuario.Status = StatusUsuario.Ativo;

            await _context.SaveChangesAsync();
        }
    }
}
