using Microsoft.AspNetCore.Mvc;
using PlayZoneApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using PlayZoneApi.DTOs.ProductoDTTOs;

namespace PlayZoneApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductoController(IProductoService service)
        {
            _service = service;
        }

        // LISTAR Y FILTRAR PRODUCTOS
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos([FromQuery] ProductoFiltroDTO filtros)
        {
            try
            {
                var productos = await _service.ObtenerTodosAsync(filtros);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al obtener los productos.", error = ex.Message });
            }
        }

        // OBTENER PRODUCTO POR ID
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> ObtenerPorId([FromRoute] int id)
        {
            try
            {
                var producto = await _service.ObtenerPorIdAsync(id);

                if (producto == null)
                    return NotFound(new { mensaje = "Producto no encontrado." });

                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al obtener el producto.", error = ex.Message });
            }
        }

        // AGREGAR PRODUCTO
        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] ProductoCrearDTO dto)
        {
            try
            {
                var id = await _service.AgregarAsync(dto);

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id },
                    new
                    {
                        mensaje = "Producto creado correctamente.",
                        id
                    });
        }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al crear el producto.", error = ex.Message });
            }
        }

        // ACTUALIZAR PRODUCTO COMPLETO
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Actualizar([FromRoute] int id, [FromBody] ProductoCrearDTO dto)
        {
            try
            {
                var actualizado = await _service.ActualizarAsync(id, dto);

                if (!actualizado)
                    return NotFound(new { mensaje = "Producto no encontrado." });

                return Ok(new { mensaje = "Producto actualizado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al actualizar el producto.", error = ex.Message });
            }
        }

        // ACTUALIZAR PRODUCTO PARCIALMENTE
        [HttpPatch]
        [Route("{id:int}")]
        public async Task<IActionResult> ActualizarParcial([FromRoute] int id, [FromBody] ProductoActualizarDTO dto)
        {
            try
            {
                var actualizado = await _service.ActualizarParcialAsync(id, dto);

                if (!actualizado)
                    return NotFound(new { mensaje = "Producto no encontrado." });

                return Ok(new { mensaje = "Producto actualizado parcialmente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al actualizar parcialmente el producto.", error = ex.Message });
            }
        }

        // ELIMINAR PRODUCTO
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Eliminar([FromRoute] int id)
        {
            try
            {
                var eliminado = await _service.EliminarAsync(id);

                if (!eliminado)
                    return NotFound(new { mensaje = "Producto no encontrado." });

                return Ok(new { mensaje = "Producto eliminado correctamente." });
        }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al eliminar el producto.", error = ex.Message });
            }
        }
    }
}