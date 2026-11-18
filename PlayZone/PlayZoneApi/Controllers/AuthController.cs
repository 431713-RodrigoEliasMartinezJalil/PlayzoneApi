using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlayZoneApi.DTOs.UsuarioDTOs;
using PlayZoneApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlayZoneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly IConfiguration _configuration;

        public AuthController(IUsuarioService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO login)
        {
            try
            {
                // Validamos que hayan llegado las credenciales
                if (login == null ||
                    string.IsNullOrWhiteSpace(login.Email) ||
                    string.IsNullOrWhiteSpace(login.Password))
                {
                    return BadRequest(new
                    {
                        message = "Debe ingresar email y contraseña."
                    });
                }

                // Autenticamos al usuario mediante el Service
                var usuario = await _service.AutenticarAsync(login);

                if (usuario == null)
                {
                    return Unauthorized(new
                    {
                        message = "Email o contraseña incorrectos."
                    });
                }

                // Creamos los Claims
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,
                        usuario.id_usuario.ToString()),

                    new Claim(ClaimTypes.Name,
                        usuario.nombre_usuario),

                    new Claim(ClaimTypes.Email,
                        usuario.email)
                };

                // Obtenemos la configuración del JWT
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _configuration["Jwt:Key"]!
                    )
                );

                var credentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );

                // Creamos el token
                var expiration = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(
                        _configuration["Jwt:ExpirationMinutes"]
                    )
                );

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: expiration,
                    signingCredentials: credentials
                );

                // Convertimos el token a texto
                var tokenString = new JwtSecurityTokenHandler()
                    .WriteToken(token);

                // Devolvemos el token
                return Ok(new
                {
                    token = tokenString,
                    expiration = expiration,
                    tokenType = "Bearer"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }
    }
}