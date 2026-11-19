using PlayZoneApi.DTOs.DashboardDTOs;

namespace PlayZoneApi.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResumenDTO> ObtenerResumenAsync();

        Task<List<IngresoMensualDTO>> ObtenerIngresosMensualesAsync(int anio);
        Task<List<VideojuegoMasVendidoDTO>> ObtenerVideojuegosMasVendidosAsync(int limite);
        Task<List<ClienteMayorGastoDTO>> ObtenerClientesMayorGastoAsync(int limite);
    }
}
