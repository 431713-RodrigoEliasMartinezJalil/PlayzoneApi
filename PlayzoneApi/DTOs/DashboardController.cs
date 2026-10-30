using Microsoft.AspNetCore.Mvc;
using PlayzoneApi.Repositories;
using PlayzoneApi.Repositories.Interfaces;

namespace PlayzoneApi.DTOs
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;
        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("resumen-ventas")]
        public async Task<IActionResult> ObtenerResumenVentasAnioActual()
        {
            return Ok(await _dashboardRepository.ObtenerResumenVentasAnioActualAsync());
        }

        [HttpGet("ingresos-mensuales")]
        public async Task<IActionResult> ObtenerIngresosMensuales([FromQuery] int anio = 2026)
        {
            return Ok(await _dashboardRepository.ObtenerIngresosMensualesAsync(anio));
        }
    }
}
