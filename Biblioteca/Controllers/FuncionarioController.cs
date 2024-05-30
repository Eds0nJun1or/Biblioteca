using AutoMapper;
using Biblioteca.Data;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly BibliotecaContext _context;
        private readonly IFuncionarioService _funcionarioService;
        private readonly string _chaveSecreta;
        private readonly IMapper _mapper;

        public FuncionarioController(BibliotecaContext context, IFuncionarioService funcionarioService, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _funcionarioService = funcionarioService;
            _chaveSecreta = configuration["ChaveSecreta"] ?? throw new ArgumentNullException(nameof(configuration), "ChaveSecreta não foi encontrada na configuração.");
            _mapper = mapper;
        }

        /// <summary>
        /// Realiza o login do funcionário e gera um token JWT.
        /// </summary>
        /// <param name="login">Objeto DTO contendo as credenciais de login.</param>
        /// <returns>Retorna o token JWT gerado.</returns>
        /// <response code="200">Retorna o token JWT gerado.</response>
        /// <response code="400">Retorna mensagem de erro se as credenciais forem inválidas.</response>
        [HttpPost]
        [Route("/Funcionario/login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(f => f.Cpf == login.Cpf && f.Senha == login.Senha);

            if (funcionario == null)
            {
                return BadRequest(new { mensagem = "Credenciais inválidas. Por favor, verifique seu CPF e senha." });
            }

            var token = GerarTokenJwt(funcionario);
            return Ok( token );
        }

        private string GerarTokenJwt(Funcionario funcionario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, funcionario.Cpf),
                new Claim(ClaimTypes.Name, funcionario.NomeFuncionario)
            };

            var token = new JwtSecurityToken(
              issuer: "Biblioteca",
              audience: "API",
              claims: claims,
              expires: DateTime.Now.AddMinutes(20),
              signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_chaveSecreta)),
                SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Retorna uma lista de todos os funcionários.
        /// </summary>
        /// <returns>Lista de funcionários.</returns>
        /// <response code="200">Retorna a lista de funcionários.</response>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(IEnumerable<ReadFuncionarioDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarTodosFuncionariosAsync()
        {
            var funcionarios = await _funcionarioService.BuscarTodosFuncionariosAsync();
            var funcionariosDto = _mapper.Map<IEnumerable<ReadFuncionarioDto>>(funcionarios);
            return Ok(funcionariosDto);
        }

        /// <summary>
        /// Retorna os detalhes de um funcionário específico pelo ID.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário.</param>
        /// <returns>Detalhes do funcionário.</returns>
        /// <response code="200">Retorna os detalhes do funcionário.</response>
        /// <response code="404">Retorna se o funcionário não for encontrado.</response>
        [HttpGet("{funcionarioId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(ReadFuncionarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarFuncionarioPorIdAsync(int funcionarioId)
        {
            var funcionario = await _funcionarioService.BuscarFuncionarioPorIdAsync(funcionarioId);

            if (funcionario == null)
            {
                return NotFound();
            }

            var funcionarioDto = _mapper.Map<ReadFuncionarioDto>(funcionario);
            return Ok(funcionarioDto);
        }

        /// <summary>
        /// Cria um novo funcionário.
        /// </summary>
        /// <param name="funcionario">Objeto contendo os dados do funcionário a ser criado.</param>
        /// <returns>Retorna o objeto do funcionário criado.</returns>
        /// <response code="201">Retorna o objeto do funcionário criado e a URL para acessá-lo.</response>
        /// <response code="400">Retorna mensagem de erro se algum dado duplicado for encontrado.</response>
        /// <response code="500">Retorna mensagem de erro se ocorrer um erro interno no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Funcionario), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarFuncionario([FromBody] Funcionario funcionario)
        {
            try
            {
                var novoFuncionario = await _funcionarioService.CriarFuncionarioAsync(funcionario);
                return CreatedAtRoute("GetFuncionarioById", new { id = novoFuncionario.FuncionarioId }, novoFuncionario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza os dados de um funcionário específico pelo ID.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário a ser atualizado.</param>
        /// <param name="funcionarioDto">Objeto DTO contendo os novos dados do funcionário.</param>
        /// <returns>Nenhum conteúdo.</returns>
        /// <response code="204">Indica que o funcionário foi atualizado com sucesso.</response>
        /// <response code="400">Retorna mensagem de erro se os dados forem inválidos.</response>
        /// <response code="404">Retorna se o funcionário não for encontrado.</response>
        [HttpPut("{funcionarioId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarFuncionarioAsync(int funcionarioId, [FromBody] UpdateFuncionarioDto funcionarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (funcionarioId != funcionarioDto.FuncionarioId)
            {
                return BadRequest("O ID do funcionário na URL não corresponde ao ID fornecido no corpo da solicitação.");
            }

            var funcionario = await _funcionarioService.BuscarFuncionarioPorIdAsync(funcionarioId);

            if (funcionario == null)
            {
                return NotFound("Funcionário não encontrado.");
            }

            // Atualiza apenas os campos que foram alterados
            funcionario.Senha = funcionarioDto.Senha;
            funcionario.Email = funcionarioDto.Email;
            funcionario.Telefone = funcionarioDto.Telefone;
            funcionario.Status = funcionarioDto.Status;

            await _funcionarioService.AtualizarFuncionarioAsync(funcionario);

            return NoContent();
        }


        /// <summary>
        /// Exclui um funcionário específico pelo ID.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário a ser excluído.</param>
        /// <returns>Nenhum conteúdo.</returns>
        /// <response code="204">Indica que o funcionário foi excluído com sucesso.</response>
        /// <response code="404">Retorna se o funcionário não for encontrado.</response>
        [HttpDelete("{funcionarioId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExcluirFuncionarioAsync(int funcionarioId)
        {
            await _funcionarioService.ExcluirFuncionarioAsync(funcionarioId);

            return NoContent();
        }
    }
}

