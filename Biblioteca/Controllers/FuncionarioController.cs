using Biblioteca.Data;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Enums;
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

        public FuncionarioController(BibliotecaContext context, IFuncionarioService funcionarioService, IConfiguration configuration)
        {
            _context = context;
            _funcionarioService = funcionarioService;
            _chaveSecreta = configuration["ChaveSecreta"] ?? throw new ArgumentNullException(nameof(configuration), "ChaveSecreta não foi encontrada na configuração.");
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
            return Ok(new { token });
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
              expires: DateTime.Now.AddMinutes(5),
              signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_chaveSecreta)),
                SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Busca todos os funcionários.
        /// </summary>
        /// <returns>Retorna uma lista de objetos de funcionários.</returns>
        /// <response code="200">Retorna a lista de funcionários.</response>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(IEnumerable<Funcionario>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarTodosFuncionariosAsync()
        {
            var funcionarios = await _funcionarioService.BuscarTodosFuncionariosAsync();
            return Ok(funcionarios);
        }

        /// <summary>
        /// Busca um funcionário pelo ID.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário.</param>
        /// <returns>Retorna o objeto do funcionário encontrado.</returns>
        /// <response code="200">Retorna o objeto do funcionário encontrado.</response>
        /// <response code="404">Retorna mensagem de erro se o funcionário não for encontrado.</response>
        [HttpGet("{funcionarioId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(Funcionario), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarFuncionarioPorIdAsync(int funcionarioId)
        {
            var funcionario = await _funcionarioService.BuscarFuncionarioPorIdAsync(funcionarioId);

            if (funcionario == null)
            {
                return NotFound();
            }

            return Ok(funcionario);
        }

        /// <summary>
        /// Cria um novo funcionário.
        /// </summary>
        /// <param name="funcionarioDto">Objeto DTO contendo os dados do funcionário a ser criado.</param>
        /// <returns>Retorna o objeto do funcionário criado e a URL para acessá-lo.</returns>
        /// <response code="201">Retorna o objeto do funcionário criado e a URL para acessá-lo.</response>
        /// <response code="400">Retorna mensagem de erro se os dados do funcionário forem inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Funcionario), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarFuncionarioAsync([FromBody] CreateFuncionarioDto funcionarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var funcionario = new Funcionario
            {
                Cpf = funcionarioDto.Cpf,
                Senha = funcionarioDto.Senha,
                NomeFuncionario = funcionarioDto.NomeFuncionario,
                DataNascimento = funcionarioDto.DataNascimento,
                Email = funcionarioDto.Email,
                Telefone = funcionarioDto.Telefone,
                Status = StatusFuncionario.Ativo // Assume o funcionário como ativo por padrão
            };

            await _funcionarioService.CriarFuncionarioAsync(funcionario);

            return CreatedAtRoute(
                nameof(BuscarFuncionarioPorIdAsync),
                new { funcionarioId = funcionario.FuncionarioId },
                funcionario);
        }

        /// <summary>
        /// Atualiza um funcionário existente.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário a ser atualizado.</param>
        /// <param name="funcionarioDto">Objeto DTO contendo os novos dados do funcionário.</param>
        /// <returns>Retorna NoContent se a atualização for bem-sucedida.</returns>
        /// <response code="204">Retorna NoContent se a atualização for bem-sucedida.</response>
        /// <response code="400">Retorna mensagem de erro se os dados do funcionário forem inválidos ou se o ID não coincidir.</response>
        /// <response code="404">Retorna mensagem de erro se o funcionário não for encontrado.</response>
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
                return BadRequest();
            }

            var funcionario = await _funcionarioService.BuscarFuncionarioPorIdAsync(funcionarioId);

            if (funcionario == null)
            {
                return NotFound();
            }

            funcionario.Senha = funcionarioDto.Senha;
            funcionario.NomeFuncionario = funcionarioDto.NomeFuncionario;
            funcionario.DataNascimento = funcionarioDto.DataNascimento;
            funcionario.Email = funcionarioDto.Email;
            funcionario.Telefone = funcionarioDto.Telefone;

            await _funcionarioService.AtualizarFuncionarioAsync(funcionario);

            return NoContent();
        }

        /// <summary>
        /// Exclui um funcionário pelo ID.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário a ser excluído.</param>
        /// <returns>Retorna NoContent se a exclusão for bem-sucedida.</returns>
        /// <response code="204">Retorna NoContent se a exclusão for bem-sucedida.</response>
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


