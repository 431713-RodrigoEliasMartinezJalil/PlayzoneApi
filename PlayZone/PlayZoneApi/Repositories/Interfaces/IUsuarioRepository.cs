using PlayZoneApi.Models;

namespace PlayZoneApi.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmailAsync(string email);
    }
}
