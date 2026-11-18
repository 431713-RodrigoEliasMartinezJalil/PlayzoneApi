using PlayZoneApi.DTOs.DashboardDTOs;
using PlayZoneApi.Repositories.Interfaces;
using PlayZoneApi.Services.Interfaces;

namespace PlayZoneApi.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        // OBTENER RESUMEN GENERAL DEL DASHBOARD
        public async Task<DashboardResumenDTO> ObtenerResumenAsync()
        {
            var resumen = new DashboardResumenDTO();

            // RESUMEN DE ORDENES
            resumen.TotalOrdenes =
                await _repository.ObtenerTotalOrdenesAsync();

            resumen.OrdenesPendientes =
                await _repository.ObtenerOrdenesPorEstadoAsync("Pendiente");

            resumen.OrdenesCompletadas =
                await _repository.ObtenerOrdenesPorEstadoAsync("Completada");

            // INFORMACION GENERAL
            resumen.TotalProductos =
                await _repository.ObtenerTotalProductosAsync();

            resumen.TotalUsuarios =
                await _repository.ObtenerTotalUsuariosAsync();

            // REEMBOLSOS
            resumen.TotalReembolsos =
                await _repository.ObtenerTotalReembolsosAsync();

            return resumen;
        }
    }
}

