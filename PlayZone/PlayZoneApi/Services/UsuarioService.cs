using PlayZoneApi.DTOs.UsuarioDTOs;
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

        public async Task<Usuario> RegistrarAsync(UserRegisterDTO registro)
        {
            var email = registro.Email.Trim();
            var nombreUsuario = registro.Username.Trim();

            if (await _repository.ObtenerPorEmailAsync(email) != null)
            {
                throw new InvalidOperationException("El email ya está registrado.");
            }

            if (await _repository.ExisteNombreUsuarioAsync(nombreUsuario))
            {
                throw new InvalidOperationException("El nombre de usuario ya está registrado.");
            }

            var pais = await _repository.ObtenerPaisPorIdAsync(registro.CountryId);
            if (pais == null)
            {
                throw new ArgumentException("El país seleccionado no existe.");
            }

            var estado = await _repository.ObtenerEstadoPorNombreAsync("Activa");
            if (estado == null)
            {
                throw new InvalidOperationException("No existe el estado de cuenta Activa.");
            }

            var usuario = new Usuario
            {
                nombre_usuario = nombreUsuario,
                email = email,
                nombre = registro.Name.Trim(),
                apellido = registro.LastName.Trim(),
                password_hash = BCrypt.Net.BCrypt.HashPassword(registro.Password),
                fecha_nacimiento = registro.BirthDate,
                id_pais = pais.id_pais,
                id_estado_cuenta = estado.id_estado_cuenta,
                fecha_registro = DateTime.UtcNow
            };

            await _repository.CrearAsync(usuario);
            return usuario;
        }
    }
}
