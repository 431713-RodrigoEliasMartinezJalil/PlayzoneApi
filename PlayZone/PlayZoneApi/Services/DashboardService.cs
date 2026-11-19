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
            return await _repository.ObtenerResumenAsync();
        }

        public async Task<List<IngresoMensualDTO>> ObtenerIngresosMensualesAsync(int anio)
        {
            return await _repository.ObtenerIngresosMensualesAsync(anio);
        }

        public async Task<List<VideojuegoMasVendidoDTO>> ObtenerVideojuegosMasVendidosAsync(int limite)
        {
            return await _repository.ObtenerVideojuegosMasVendidosAsync(limite);
        }

        public async Task<List<ClienteMayorGastoDTO>> ObtenerClientesMayorGastoAsync(int limite)
        {
            return await _repository.ObtenerClientesMayorGastoAsync(limite);
        }

    }
}

