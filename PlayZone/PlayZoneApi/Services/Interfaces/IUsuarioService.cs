using PlayZoneApi.DTOs;
using PlayZoneApi.Models;

namespace PlayZoneApi.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> AutenticarAsync(UserLoginDTO login);
    }
}
