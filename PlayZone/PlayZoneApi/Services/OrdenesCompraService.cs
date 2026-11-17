
using PlayZoneApi.DTOs;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;
using PlayZoneApi.Services.Interfaces;

namespace PlayZoneApi.Services
{
    public class OrdenesCompraService : IOrdenesCompraService
    {
        private readonly IOrdenesCompraRepository _repository;

        public OrdenesCompraService(IOrdenesCompraRepository repository)
        {
            _repository = repository;
        }

        // =========================================
        // OBTENER TODAS LAS ORDENES
        // =========================================

        public async Task<List<OrdenCompraDTO>> ObtenerTodasAsync()
        {
            var ordenes = await _repository.ObtenerTodasAsync();

            var listaDTO = new List<OrdenCompraDTO>();

            foreach (var orden in ordenes)
            {
                listaDTO.Add(MapearOrden(orden));
            }

            return listaDTO;
        }

        // =========================================
        // OBTENER ORDEN POR ID
        // =========================================

        public async Task<OrdenCompraDTO> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la orden debe ser mayor a cero.");

            var orden = await _repository.ObtenerPorIdAsync(id);

            if (orden == null)
                throw new KeyNotFoundException("No se encontró la orden de compra.");

            return MapearOrden(orden);
        }

        // =========================================
        // OBTENER ORDENES POR USUARIO
        // =========================================

        public async Task<List<OrdenCompraDTO>> ObtenerPorUsuarioAsync(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID del usuario debe ser mayor a cero.");

            var ordenes = await _repository.ObtenerPorUsuarioAsync(idUsuario);

            var listaDTO = new List<OrdenCompraDTO>();

            foreach (var orden in ordenes)
            {
                listaDTO.Add(MapearOrden(orden));
            }

            return listaDTO;
        }

        // =========================================
        // MAPEAR ORDEN A DTO
        // =========================================

        private static OrdenCompraDTO MapearOrden(Ordenes_Compra orden)
        {
            var dto = new OrdenCompraDTO
            {
                IdOrden = orden.id_orden,
                IdUsuario = orden.id_usuario,
                NombreUsuario = orden.id_usuarioNavigation.nombre_usuario,
                EmailUsuario = orden.id_usuarioNavigation.email,
                FechaOrden = orden.fecha_orden,
                MetodoPago = orden.id_metodo_pagoNavigation.nombre,
                Moneda = orden.id_monedaNavigation.nombre,
                CodigoMoneda = orden.id_monedaNavigation.codigo,
                EstadoOrden = orden.id_estado_ordenNavigation.nombre,

                Detalles = new List<DetalleOrdenDTO>()
            };

            foreach (var detalle in orden.Detalle_Ordens)
            {
                var detalleDTO = new DetalleOrdenDTO
                {
                    IdProducto = detalle.id_producto,
                    TituloProducto = detalle.id_productoNavigation.titulo,
                    PrecioUnitario = detalle.precio_unitario,
                    MontoDescuento = detalle.monto_descuento,
                    EsRegalo = detalle.es_regalo
                };

                dto.Detalles.Add(detalleDTO);
            }

            return dto;
        }
    }
}
