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
            // Verificar se o empréstimo existe e está atrasado
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Exemplar)
                    .ThenInclude(ex => ex.Livro)
                .FirstOrDefaultAsync(e => e.EmprestimoId == multaDto.EmprestimoId && e.Status == StatusEmprestimo.Devolvido);

            if (emprestimo == null)
            {
                throw new Exception("Empréstimo não encontrado ou não possui exemplar associado.");
            }

            if (emprestimo.Exemplar == null)
            {
                throw new Exception("Empréstimo encontrado, mas não possui exemplar associado.");
            }

            // Calcular a multa
            double valorLivro = emprestimo.Exemplar.Livro.Valor; // Supondo que há uma propriedade Valor no modelo Livro
            double valorMulta = multaDto.DiasAtrasados * 1.0; // R$ 1 por dia
            valorMulta = Math.Min(valorMulta, valorLivro * 2); // Não exceder o dobro do valor do livro

            var multa = _mapper.Map<Multa>(multaDto);
            multa.Emprestimo = emprestimo;
            multa.Usuario = emprestimo.Usuario;
            multa.Valor = valorMulta;
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
            multaDto.Id = multa.MultaId;
            multaDto.NomeUsuario = multa.Emprestimo.Usuario.Nome;
            multaDto.UsuarioId = multa.Emprestimo.Usuario.UsuarioId;
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

            if (multas == null || !multas.Any())
            {
                throw new NotFoundException("Nenhuma multa encontrada.");
            }

            var multasDto = new List<ReadMultaDto>();

            foreach (var multa in multas)
            {
                var multaDto = new ReadMultaDto
                {
                    Id = multa.MultaId,
                    EmprestimoId = multa.EmprestimoId,
                    Valor = multa.Valor,
                    InicioMulta = multa.InicioMulta,
                    FimMulta = multa.FimMulta,
                    DiasAtrasados = multa.DiasAtrasados,
                    Status = multa.Status,
                    NomeUsuario = multa.Emprestimo.Usuario.Nome,
                    TituloLivro = multa.Emprestimo.Exemplar.Livro.Nome,
                    UsuarioId = multa.Emprestimo.Usuario.UsuarioId
                };

                multasDto.Add(multaDto);
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
