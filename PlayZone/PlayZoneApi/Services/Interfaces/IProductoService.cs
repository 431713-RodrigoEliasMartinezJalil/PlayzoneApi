using PlayZoneApi.DTOs.ProductoDTTOs;

namespace PlayZoneApi.Services.Interfaces
{
    public interface IProductoService
    {
        Task<List<ProductoRespuestaDTO>> ObtenerTodosAsync(
            ProductoFiltroDTO filtros);

        Task<ProductoRespuestaDTO?> ObtenerPorIdAsync(int id);

        Task<int> AgregarAsync(ProductoCrearDTO dto);

        Task<bool> ActualizarAsync(int id, ProductoCrearDTO dto);

        Task<bool> ActualizarParcialAsync(int id, ProductoActualizarDTO dto);

        Task<bool> EliminarAsync(int id);
    }
}