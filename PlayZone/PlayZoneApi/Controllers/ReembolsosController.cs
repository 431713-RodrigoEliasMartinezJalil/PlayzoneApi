using Microsoft.AspNetCore.Mvc;
using PlayZoneApi.DTOs.ReembolsoDTOs;
using PlayZoneApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PlayZoneApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReembolsosController : ControllerBase
    {
        private readonly IReembolsosService _service;

        public ReembolsosController(IReembolsosService service)
        {
            _service = service;
        }

        // =========================================
        // CREAR SOLICITUD DE REEMBOLSO
        // =========================================

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ReembolsoCrearDTO dto)
        {
            try
            {
                var reembolsoCreado = await _service.CrearAsync(dto);

                return Ok(reembolsoCreado);
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
        // OBTENER TODOS LOS REEMBOLSOS
        // =========================================

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var reembolsos = await _service.ObtenerTodosAsync();

                return Ok(reembolsos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var reembolso = await _service.ObtenerPorIdAsync(id);
                return Ok(reembolso);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ReembolsoActualizarDTO dto)
        {
            try
            {
                var reembolsoActualizado = await _service.ActualizarAsync(id, dto);
                return Ok(reembolsoActualizado);
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

    }
}
