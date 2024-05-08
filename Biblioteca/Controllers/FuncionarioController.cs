//using Biblioteca.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace Biblioteca.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class FuncionarioController : ControllerBase
//    {
//        [HttpPost]
//        public IActionResult Login([FromBody] Funcionario funcionario)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (funcionario.Login == "admin" && funcionario.Senha == "admin123")
//            {
//                var token = GerarTokenJwt();
//                return Ok(new { token });
//            }

//            return BadRequest(new { mensagem = "Credenciais inválidas. Por favor, verifique seu nome de usuário e senha." });
//        }

//        private string GerarTokenJwt()
//        {
//            string chaveSecreta = "d35b3fca-e895-4c2d-a6f5-24c640bea640";

//            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta));
//            var credencial = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//            {
//                new Claim("Login", "admin"),
//                new Claim("nome", "Funcionário Biblioteca")
//            };

//            var token = new JwtSecurityToken(
//                issuer: "Biblioteca",
//                audience: "API",
//                claims: null,
//                expires: DateTime.Now.AddMinutes(10),
//                signingCredentials: credencial
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}

