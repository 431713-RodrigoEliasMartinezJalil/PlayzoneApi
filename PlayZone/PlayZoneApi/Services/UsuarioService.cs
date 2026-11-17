using PlayZoneApi.DTOs;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;
using PlayZoneApi.Services.Interfaces;

namespace PlayZoneApi.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<Usuario?> AutenticarAsync(UserLoginDTO login)
        {
            // Buscamos al usuario por email
            Usuario? usuario = await _repository.ObtenerPorEmailAsync(login.Email);

            if (usuario == null)
                return null;

            // Verificamos la contraseña con BCrypt
            bool passwordValida = BCrypt.Net.BCrypt.Verify(
                login.Password,
                usuario.password_hash
            );

            if (!passwordValida)
                return null;

            // Comprobamos que la cuenta esté activa
            if (usuario.id_estado_cuentaNavigation.estado != "Activa")
                return null;

            return usuario;
        }
    }
}
