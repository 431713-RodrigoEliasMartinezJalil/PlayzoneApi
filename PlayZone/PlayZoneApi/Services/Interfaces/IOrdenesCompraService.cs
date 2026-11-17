using PlayZoneApi.DTOs;

namespace PlayZoneApi.Services.Interfaces
{
    public interface IOrdenesCompraService
    {
        Task<List<OrdenCompraDTO>> ObtenerTodasAsync();

        Task<OrdenCompraDTO> ObtenerPorIdAsync(int id);

        Task<List<OrdenCompraDTO>> ObtenerPorUsuarioAsync(int idUsuario);
    }
}
