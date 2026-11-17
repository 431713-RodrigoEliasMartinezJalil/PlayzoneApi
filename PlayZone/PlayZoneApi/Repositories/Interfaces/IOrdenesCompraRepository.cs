using PlayZoneApi.Models;

namespace PlayZoneApi.Repositories.Interfaces
{
    public interface IOrdenesCompraRepository
    {
        Task<List<Ordenes_Compra>> ObtenerTodasAsync();
        Task<Ordenes_Compra?> ObtenerPorIdAsync(int id);
        Task<List<Ordenes_Compra>> ObtenerPorUsuarioAsync(string nombreUsuario);
    }
}
