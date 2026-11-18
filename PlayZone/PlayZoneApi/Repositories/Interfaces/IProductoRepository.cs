using PlayZoneApi.DTOs.ProductoDTTOs;
using PlayZoneApi.Models;

namespace PlayZoneApi.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        Task<List<Producto>> ObtenerTodosAsync(ProductoFiltroDTO filtros);
        Task<Producto?> ObtenerPorIdAsync(int id);

        Task<Tipos_Producto?> ObtenerTipoProductoAsync(int id);
        Task<Estados_Publicacion?> ObtenerEstadoPublicacionAsync(int id);

        Task AgregarAsync(Producto producto);
        Task ActualizarAsync(Producto producto);
        Task EliminarAsync(Producto producto);
    }
}
