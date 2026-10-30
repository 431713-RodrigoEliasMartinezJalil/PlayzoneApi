using Microsoft.AspNetCore.Mvc;
using PlayzoneApi.DTOs;
using PlayzoneApi.Models;
using PlayzoneApi.Repositories;
using PlayzoneApi.Repositories.Interfaces;

namespace PlayzoneApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoporteController : ControllerBase
    {
        private readonly ISoporteRepository _soporteRepository;

        public SoporteController(ISoporteRepository soporteRepository)
        {
            _soporteRepository = soporteRepository;
        }

        [HttpPost("reembolsos")]
        public async Task<IActionResult> SolicitarReembolso([FromBody] SolicitudReembolsoRequestDto request)
        {
            var detalle = await _soporteRepository.ObtenerDetalleOrdenConProductoAsync(request.IdDetalleOrden, request.IdUsuario);
            if (detalle == null) return NotFound(new { mensaje = "Ítem de compra no encontrado." });

            if (await _soporteRepository.ExisteReembolsoParaDetalleAsync(request.IdDetalleOrden))
            {
                return BadRequest(new { mensaje = "Ya existe un trámite de reembolso para este producto." });
            }

            var reembolso = new Reembolso
            {
                IdDetalleOrden = request.IdDetalleOrden,
                IdEstadoReembolso = 1,
                FechaSolicitud = DateTime.UtcNow,
                Motivo = request.Motivo,
                MontoReembolsado = detalle.PrecioUnitario - detalle.MontoDescuento
            };

            await _soporteRepository.RegistrarSolicitudReembolsoAsync(reembolso);
            return Ok(new { mensaje = "Solicitud de reembolso registrada con éxito." });
        }

        [HttpPost("reseñas")]
        public async Task<IActionResult> PublicarReseña([FromBody] CrearReseñaRequestDto request)
        {
            if (!await _soporteRepository.UsuarioPoseeProductoEnBibliotecaAsync(request.IdUsuario, request.IdProducto))
            {
                return BadRequest(new { mensaje = "Solo los usuarios que poseen el juego pueden reseñarlo." });
            }

            if (await _soporteRepository.ObtenerReseñaExistenteAsync(request.IdUsuario, request.IdProducto) != null)
            {
                return BadRequest(new { mensaje = "Ya has publicado una reseña para este videojuego." });
            }

            var reseña = new Reseña
            {
                IdUsuario = request.IdUsuario,
                IdProducto = request.IdProducto,
                EsRecomendado = request.EsRecomendado,
                Comentario = request.Comentario,
                FechaPublicacion = DateTime.UtcNow
            };

            await _soporteRepository.PublicarReseñaAsync(reseña);
            return Ok(new { mensaje = "Reseña publicada con éxito." });
        }
    }
}
