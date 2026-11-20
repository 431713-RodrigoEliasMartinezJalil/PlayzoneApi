using PlayZoneApi.Models;

namespace PlayZoneApi.Repositories.Interfaces
{
    public interface IOrdenesCompraRepository
    {
        Task<List<Ordenes_Compra>> ObtenerTodasAsync();

        Task<Ordenes_Compra?> ObtenerPorIdAsync(int id);

        Task<List<Ordenes_Compra>> ObtenerPorUsuarioAsync(string nombreUsuario);

        Task<Ordenes_Compra> CrearAsync(Ordenes_Compra orden);

        Task<Estados_Orden?> ObtenerEstadoOrdenAsync(string nombre);

        Task<bool> ExisteUsuarioAsync(int id);

        Task<bool> ExisteMetodoPagoAsync(int id);

        Task<bool> ExisteMonedaAsync(int id);

        Task<bool> ExistenProductosAsync(List<int> ids);

        Task<List<Precios_Producto>> ObtenerPreciosVigentesAsync(List<int> idsProductos, int idMoneda, DateTime fecha);

        Task<Ordenes_Compra?> ObtenerPorIdParaActualizarAsync(int id);

        Task ActualizarAsync(Ordenes_Compra orden);
        Task<bool> CrearCompletoAsync(int id, Ordenes_Compra orden);
    }
}