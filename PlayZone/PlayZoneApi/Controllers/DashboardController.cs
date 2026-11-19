using Microsoft.AspNetCore.Authorization;
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

        // =========================================
        // OBTENER RESUMEN DEL DASHBOARD
        // =========================================

        // GET: api/Dashboard/resumen
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
                Console.WriteLine(ex.ToString());

                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al obtener el resumen del dashboard."
                });
            }
        }

        [HttpGet("ingresos-mensuales")]
        public async Task<IActionResult> ObtenerIngresosMensuales([FromQuery] int anio)
        {
            try
            {
                if (anio < 2000 || anio > DateTime.Now.Year)
                {
                    return BadRequest(new
                    {
                        mensaje = "El año ingresado no es válido."
                    });
                }

                var ingresos = await _service.ObtenerIngresosMensualesAsync(anio);

                return Ok(ingresos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al obtener los ingresos mensuales."
                });
            }
        }


        [HttpGet("videojuegos-mas-vendidos")]
        public async Task<IActionResult> ObtenerVideojuegosMasVendidos([FromQuery] int limite = 10)
        {
            try
            {
                if (limite < 1 || limite > 100)
                {
                    return BadRequest(new
                    {
                        mensaje = "El límite debe estar entre 1 y 100."
                    });
                }

                var videojuegos = await _service.ObtenerVideojuegosMasVendidosAsync(limite);

                return Ok(videojuegos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al obtener los videojuegos más vendidos."
                });
            }
        }

        [HttpGet("clientes-mayor-gasto")]
        public async Task<IActionResult> ObtenerClientesMayorGasto([FromQuery] int limite = 10)
        {
            try
            {
                if (limite < 1 || limite > 100)
                {
                    return BadRequest(new
                    {
                        mensaje = "El límite debe estar entre 1 y 100."
                    });
                }

                var clientes = await _service.ObtenerClientesMayorGastoAsync(limite);

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al obtener los clientes con mayor gasto."
                });
            }
        }

    }
}
