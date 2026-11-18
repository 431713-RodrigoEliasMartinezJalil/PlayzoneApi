using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayZoneApi.Services.Interfaces;

namespace PlayZoneApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        // OBTENER RESUMEN GENERAL DEL DASHBOARD
        [HttpGet("resumen")]
        public async Task<IActionResult> ObtenerResumen()
        {
            try
            {
                var resumen = await _service.ObtenerResumenAsync();

                return Ok(resumen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al obtener el resumen del Dashboard.",
                    error = ex.Message
                });
            }
        }
    }
}

