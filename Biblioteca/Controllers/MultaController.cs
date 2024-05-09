using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Biblioteca.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Enums;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
//using Microsoft.AspNetCore.Authorization;

namespace Biblioteca.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MultaController : ControllerBase
    {
        private readonly BibliotecaContext _context;
        private readonly IMapper _mapper;

        public MultaController(BibliotecaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Adiciona uma nova multa ao banco de dados.
        /// </summary>
        /// <param name="multaDto">Objeto contendo os dados da multa.</param>
        /// <returns>IActionResult</returns>
        /// <response code="201">Caso a multa seja criada com sucesso.</response>
        /// <response code="400">Caso haja algum erro na validação dos dados.</response>
        /// <response code="404">Caso o empréstimo não seja encontrado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AdicionaMulta([FromBody] CreateMultaDto multaDto)
        {
            var emprestimo = _context.Emprestimos
                .Include(e => e.Exemplar)
                .Include(e => e.Exemplar.Livro)
                .FirstOrDefault(e => e.Id == multaDto.EmprestimoId);

            if (emprestimo == null)
            {
                return NotFound("Empréstimo não encontrado.");
            }

            var diasAtrasados = DateTime.Now.Subtract(emprestimo.DataDevolucaoPrevista).Days;
            if (diasAtrasados <= 0)
            {
                return BadRequest("O empréstimo ainda não está em atraso.");
            }

            var valorMulta = CalculaValorMulta(emprestimo.Exemplar.Livro.Valor, diasAtrasados);

            Multa multa = new Multa
            {
                EmprestimoId = multaDto.EmprestimoId,
                Valor = valorMulta,
                InicioMulta = DateTime.Now,
                DiasAtrasados = diasAtrasados,
                Status = StatusMulta.Pendente
            };

            _context.Add(multa);
            _context.SaveChanges();

            return CreatedAtAction(nameof(RecuperaMultaPorId), new { id = multa.Id }, multa);
        }

        /// <summary>
        /// Calcula o valor da multa com base no valor do livro e nos dias de atraso.
        /// </summary>
        /// <param name="valorLivro">Valor do livro emprestado.</param>
        /// <param name="diasAtrasados">Quantidade de dias em atraso.</param>
        /// <returns>Valor da multa calculado.</returns>
        private float CalculaValorMulta(float valorLivro, int diasAtrasados)
        {
            float valorMultaBase = valorLivro * 0.1f;
            float valorMultaTotal = valorMultaBase * diasAtrasados;

            if (valorMultaTotal > valorLivro * 2)
            {
                valorMultaTotal = valorLivro * 2;
            }

            return valorMultaTotal;
        }

        /// <summary>
        /// Recupera uma multa específica por ID.
        /// </summary>
        /// <param name="id">ID da multa.</param>
        /// <returns>Detalhes da multa.</returns>
        /// <response code="200">Caso a multa seja encontrada.</response>
        /// <response code="404">Caso a multa não seja encontrada.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RecuperaMultaPorId(int id)
        {
            var multa = _context.Multas
                .Include(m => m.Emprestimo)
                .Include(m => m.Emprestimo.Exemplar)
                .Include(m => m.Emprestimo.Exemplar.Livro)
                .FirstOrDefault(m => m.Id == id);

            if (multa == null)
            {
                return NotFound("Multa não encontrada.");
            }

            var multaDto = _mapper.Map<ReadMultaDto>(multa);
            return Ok(multaDto);
        }

        /// <summary>
        /// Consulta todas as multas pendentes.
        /// </summary>
        /// <returns>Lista de multas pendentes.</returns>
        /// <response code="200">Caso existam multas pendentes.</response>
        /// <response code="204">Caso não existam multas pendentes.</response>
        [HttpGet("Pendentes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult ConsultaMultasPendentes()
        {
            var multasPendentes = _context.Multas
                .Where(m => m.Status == StatusMulta.Pendente)
                .ToList();

            if (multasPendentes.Count == 0)
            {
                return NoContent();
            }

            var multasDto = _mapper.Map<List<ReadMultaDto>>(multasPendentes);
            return Ok(multasDto);
        }

        /// <summary>
        /// Atualiza o status de uma multa para "Pago".
        /// </summary>
        /// <param name="id">ID da multa.</param>
        /// <returns>NoContent caso a atualização seja bem-sucedida, NotFound caso a multa não seja encontrada.</returns>
        /// <response code="204">Caso o status da multa seja atualizado para "Pago".</response>
        /// <response code="404">Caso a multa não seja encontrada.</response>
        [HttpPut("{id}/Pagar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult MarcarMultaComoPaga(int id)
        {
            var multa = _context.Multas.FirstOrDefault(m => m.Id == id);

            if (multa == null)
            {
                return NotFound("Multa não encontrada.");
            }

            if (multa.Status != StatusMulta.Pendente)
            {
                return BadRequest("A multa não está pendente.");
            }

            multa.Status = StatusMulta.Pago;
            multa.FimMulta = DateTime.Now;

            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Cancela uma multa.
        /// </summary>
        /// <param name="id">ID da multa.</param>
        /// <returns>NoContent caso o cancelamento seja bem-sucedido, NotFound caso a multa não seja encontrada.</returns>
        /// <response code="204">Caso a multa seja cancelada.</response>
        /// <response code="404">Caso a multa não seja encontrada.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CancelarMulta(int id)
        {
            var multa = _context.Multas.FirstOrDefault(m => m.Id == id);

            if (multa == null)
            {
                return NotFound("Multa não encontrada.");
            }

            if (multa.Status != StatusMulta.Pendente)
            {
                return BadRequest("A multa não está pendente.");
            }

            multa.Status = StatusMulta.Cancelado;

            _context.SaveChanges();

            return NoContent();
        }
    }
}


