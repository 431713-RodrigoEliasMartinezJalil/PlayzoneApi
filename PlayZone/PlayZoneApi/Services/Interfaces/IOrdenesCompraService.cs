using PlayZoneApi.DTOs.OrdenDTOs;

namespace PlayZoneApi.Services.Interfaces
{
    public interface IOrdenesCompraService
    {
        Task<List<OrdenCompraDTO>> ObtenerTodasAsync();

        Task<OrdenCompraDTO> ObtenerPorIdAsync(int id);

        Task<List<OrdenCompraDTO>> ObtenerPorUsuarioAsync(string nombreUsuario);

        Task<OrdenCompraDTO> CrearAsync(OrdenCompraCrearDTO dto);

        Task<OrdenCompraDTO> ActualizarAsync(int id, OrdenCompraActualizarDTO dto);
        Task<bool> EliminarAsync(int id);
        Task<bool> CrearCompletoAsync(int id, OrdenCompraCrearDTO dto);
    }
}
