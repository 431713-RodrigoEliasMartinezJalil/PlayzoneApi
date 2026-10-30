using Microsoft.AspNetCore.Mvc;
using PlayzoneApi.DTOs;
using PlayzoneApi.Repositories;
using PlayzoneApi.Repositories.Interfaces;

namespace PlayzoneApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {

            if (string.IsNullOrWhiteSpace(request.EmailOrUsername) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { mensaje = "Debe proporcionar usuario/email y contraseña." });
            }

            var usuario = await _usuarioRepository.ObtenerPorEmailONombreUsuarioAsync(request.EmailOrUsername);

            if (usuario == null || usuario.PasswordHash != request.Password)
            {
                return Unauthorized(new { mensaje = "Credenciales inválidas." });
            }

            string estado = usuario.IdEstadoCuentaNavigation.Estado;

            if (estado == "Suspendida")
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    mensaje = "Su cuenta se encuentra suspendida. Contacte a soporte."
                });
            }

            if (estado == "Pendiente")
            {
                return Unauthorized(new
                {
                    mensaje = "Cuenta pendiente de activación. Por favor confirme su correo electrónico."
                });
            }

            var respuestaDto = new UsuarioSesionDto
            {
                IdUsuario = usuario.IdUsuario,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                Pais = usuario.IdPaisNavigation.Nombre,
                EstadoCuenta = usuario.IdEstadoCuentaNavigation.Estado,
                Nivel = usuario.NivelesUsuario?.Nivel ?? 1,
                Experiencia = usuario.NivelesUsuario?.Experiencia ?? 0,
                AvatarUrl = usuario.AvatarUrl
            };

            return Ok(respuestaDto);
        }
    }
}
