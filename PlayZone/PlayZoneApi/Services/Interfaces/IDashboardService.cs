using PlayZoneApi.DTOs.DashboardDTOs;

namespace PlayZoneApi.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResumenDTO> ObtenerResumenAsync();
    }
}
