using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayZoneApi.DTOs.OrdenDTOs;
using PlayZoneApi.Services.Interfaces;

namespace PlayZoneApi.Controllers
{
    [Authorize]
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
        [Route("usuario")]
        public async Task<IActionResult> ObtenerPorUsuario([FromQuery] string nombreUsuario)
        {
            try
            {
                var ordenes = await _service.ObtenerPorUsuarioAsync(nombreUsuario);
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

        // =========================================
        // CREAR ORDEN DE COMPRA
        // =========================================

        // POST: api/OrdenesCompra
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] OrdenCompraCrearDTO dto)
        {
            try
            {
                var ordenCreada = await _service.CrearAsync(dto);

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = ordenCreada.IdOrden },
                    ordenCreada
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // =========================================
        // ACTUALIZAR ESTADO DE ORDEN
        // =========================================

        // PUT: api/OrdenesCompra/estado/5

        [HttpPut("estado/{id}")]
        public async Task<IActionResult> Actualizar(
            int id,
            [FromBody] OrdenCompraActualizarDTO dto)
        {
            try
            {
                var ordenActualizada = await _service.ActualizarAsync(id, dto);

                return Ok(ordenActualizada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // =========================================
        // BAJA LOGICA DE ORDEN
        // =========================================

        // DELETE: api/OrdenesCompra/5

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _service.EliminarAsync(id);

                return Ok("Orden eliminada");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // =========================================
        // AZTUALIZAR COMPLETRA UNA ORDEN
        // =========================================

        // PUT: api/OrdenesCompra/5

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCompleto(int id, [FromBody] OrdenCompraCrearDTO dto)
        {
            try
            {
                bool ordenActualizada = await _service.CrearCompletoAsync(id, dto);
                
                if (ordenActualizada)
                {
                    return Ok("Orden actualizada con éxito.");
                }
                throw new InvalidOperationException("Error al actualizar.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }
    }
}
