using PlayZoneApi.Models;

namespace PlayZoneApi.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmailAsync(string email);

        Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario);

        Task<Estado_Cuentum?> ObtenerEstadoPorNombreAsync(string estado);

        Task<Pai?> ObtenerPaisPorIdAsync(int idPais);

        Task CrearAsync(Usuario usuario);
    }
}
