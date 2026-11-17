
using Microsoft.AspNetCore.Mvc;
using PlayZoneApi.Services.Interfaces;

namespace PlayZoneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesCompraController : ControllerBase
    {
        private readonly IOrdenesCompraService _service;

        public OrdenesCompraController(IOrdenesCompraService service)
        {
            _service = service;
        }

        // GET: api/OrdenesCompra
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var ordenes = await _service.ObtenerTodasAsync();

                return Ok(ordenes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // GET: api/OrdenesCompra/5
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> ObtenerPorId([FromRoute] int id)
        {
            try
            {
                var orden = await _service.ObtenerPorIdAsync(id);

                return Ok(orden);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // GET: api/OrdenesCompra/usuario/idUsuario
        [HttpGet]
        [Route("usuario/{idUsuario:int}")]
        public async Task<IActionResult> ObtenerPorUsuario([FromRoute]int idUsuario)
        {
            try
            {
                var ordenes = await _service.ObtenerPorUsuarioAsync(idUsuario);

                return Ok(ordenes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }
    }
}
