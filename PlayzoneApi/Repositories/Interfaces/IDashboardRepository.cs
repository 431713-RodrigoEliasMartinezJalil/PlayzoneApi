using PlayzoneApi.DTOs;

namespace PlayzoneApi.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<ResumenVentaDto>> ObtenerResumenVentasAnioActualAsync();
        Task<List<IngresoMensualDto>> ObtenerIngresosMensualesAsync(int anio);
    }
}
