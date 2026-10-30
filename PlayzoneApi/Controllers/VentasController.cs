using Microsoft.AspNetCore.Mvc;
using PlayzoneApi.DTOs;
using PlayzoneApi.Models;
using PlayzoneApi.Repositories;
using PlayzoneApi.Repositories.Interfaces;

namespace PlayzoneApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaRepository _ventaRepository;

        public VentasController(IVentaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] CrearVentaRequestDto request)
        {
            if (request == null || !request.Items.Any())
            {
                return BadRequest(new { mensaje = "La solicitud no contiene productos." });
            }

            bool usuarioValido = await _ventaRepository.UsuarioExisteYEstaActivoAsync(request.IdUsuario);
            if (!usuarioValido)
            {
                return BadRequest(new { mensaje = "El usuario no existe o la cuenta no se encuentra activa." });
            }

            var idsParaMi = request.Items.Where(i => !i.EsRegalo).Select(i => i.IdProducto).Distinct().ToList();
            if (idsParaMi.Any())
            {
                var yaPoseidos = await _ventaRepository.ObtenerProductosEnBibliotecaAsync(request.IdUsuario, idsParaMi);
                if (yaPoseidos.Any())
                {
                    return BadRequest(new { mensaje = "El usuario ya posee en biblioteca uno o más de los productos.", duplicados = yaPoseidos });
                }
            }

            var detallesNuevos = new List<DetalleOrden>();
            var itemsBiblioteca = new List<BibliotecaUsuario>();

            foreach (var item in request.Items)
            {
                decimal? precioVigente = await _ventaRepository.ObtenerPrecioVigenteAsync(item.IdProducto, request.IdMoneda);
                if (precioVigente == null)
                {
                    return BadRequest(new { mensaje = $"Precio no disponible para producto ID {item.IdProducto}." });
                }

                detallesNuevos.Add(new DetalleOrden
                {
                    IdProducto = item.IdProducto,
                    PrecioUnitario = precioVigente.Value,
                    MontoDescuento = item.MontoDescuento,
                    EsRegalo = item.EsRegalo
                });

                if (!item.EsRegalo)
                {
                    itemsBiblioteca.Add(new BibliotecaUsuario
                    {
                        IdUsuario = request.IdUsuario,
                        IdProducto = item.IdProducto,
                        FechaAdquisicion = DateTime.UtcNow,
                        OrigenAdquisicion = "Compra_Directa"
                    });
                }
            }

            var nuevaOrden = new OrdenesCompra
            {
                IdUsuario = request.IdUsuario,
                IdMetodoPago = request.IdMetodoPago,
                IdMoneda = request.IdMoneda,
                IdEstadoOrden = 1,
                FechaOrden = DateTime.UtcNow
            };

            var ordenCreada = await _ventaRepository.ProcesarVentaTransaccionalAsync(nuevaOrden, detallesNuevos, itemsBiblioteca);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = ordenCreada.IdOrden }, new { idOrden = ordenCreada.IdOrden, estado = "Completada" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var orden = await _ventaRepository.ObtenerVentaPorIdAsync(id);
            if (orden == null) return NotFound();
            return Ok(orden);
        }
    }
}
