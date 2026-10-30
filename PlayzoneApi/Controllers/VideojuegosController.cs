using Microsoft.AspNetCore.Mvc;
using PlayzoneApi.DTOs;
using PlayzoneApi.Repositories;
using PlayzoneApi.Repositories.Interfaces;

namespace PlayzoneApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideojuegosController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;
        public VideojuegosController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCatalogoPaginado(
            [FromQuery] ProductoFiltroDto filtro,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanoPagina = 10)
        {
            if (tamanoPagina > 50) tamanoPagina = 50;
            if (pagina < 1) pagina = 1;

            var resultado = await _productoRepository.ListarCatalogoPaginadoAsync(filtro, pagina, tamanoPagina);
            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            var producto = await _productoRepository.ObtenerPorIdConRelacionesAsync(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = $"No se encontró el videojuego con ID {id}." });
            }

            return Ok(producto);
        }
    }
}
