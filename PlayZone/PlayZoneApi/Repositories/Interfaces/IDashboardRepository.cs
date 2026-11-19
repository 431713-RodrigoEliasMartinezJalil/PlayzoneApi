using PlayZoneApi.DTOs.DashboardDTOs;

namespace PlayZoneApi.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardResumenDTO> ObtenerResumenAsync();
        Task<List<IngresoMensualDTO>> ObtenerIngresosMensualesAsync(int anio);
        Task<List<VideojuegoMasVendidoDTO>> ObtenerVideojuegosMasVendidosAsync(int limite);
        Task<List<ClienteMayorGastoDTO>> ObtenerClientesMayorGastoAsync(int limite);
    }
}