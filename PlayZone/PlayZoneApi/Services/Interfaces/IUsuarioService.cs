using PlayZoneApi.DTOs.UsuarioDTOs;
using PlayZoneApi.Models;

namespace PlayZoneApi.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> AutenticarAsync(UserLoginDTO login);

        Task<Usuario> RegistrarAsync(UserRegisterDTO registro);
    }
}
