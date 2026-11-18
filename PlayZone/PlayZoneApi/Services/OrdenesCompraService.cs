using PlayZoneApi.DTOs.OrdenDTOs;
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

        public async Task<List<OrdenCompraDTO>> ObtenerPorUsuarioAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            var ordenes = await _repository.ObtenerPorUsuarioAsync(nombreUsuario);

            var listaDTO = new List<OrdenCompraDTO>();

            foreach (var orden in ordenes)
            {
                listaDTO.Add(MapearOrden(orden));
            }

            return listaDTO;
        }

        // =========================================
        // CREAR ORDEN DE COMPRA
        // =========================================

        public async Task<OrdenCompraDTO> CrearAsync(OrdenCompraCrearDTO dto)
        {
            // VALIDAR DATOS GENERALES

            if (dto == null)
                throw new ArgumentException("Los datos de la orden son obligatorios.");

            if (dto.IdUsuario <= 0)
                throw new ArgumentException("El usuario debe ser válido.");

            if (dto.IdMetodoPago <= 0)
                throw new ArgumentException("El método de pago debe ser válido.");

            if (dto.IdMoneda <= 0)
                throw new ArgumentException("La moneda debe ser válida.");

            if (dto.Detalles == null || dto.Detalles.Count == 0)
                throw new ArgumentException("La orden debe contener al menos un producto.");

            // VALIDAR EXISTENCIA DEL USUARIO

            if (!await _repository.ExisteUsuarioAsync(dto.IdUsuario))
                throw new KeyNotFoundException("No existe el usuario indicado.");

            // VALIDAR EXISTENCIA DEL MÉTODO DE PAGO

            if (!await _repository.ExisteMetodoPagoAsync(dto.IdMetodoPago))
                throw new KeyNotFoundException("No existe el método de pago indicado.");

            // VALIDAR EXISTENCIA DE LA MONEDA

            if (!await _repository.ExisteMonedaAsync(dto.IdMoneda))
                throw new KeyNotFoundException("No existe la moneda indicada.");

            // VALIDAR PRODUCTOS REPETIDOS

            var productosRepetidos = dto.Detalles
                .GroupBy(d => d.IdProducto)
                .Any(g => g.Count() > 1);

            if (productosRepetidos)
                throw new ArgumentException("No se puede agregar el mismo producto más de una vez.");

            // VALIDAR IDs Y DESCUENTOS

            foreach (var detalle in dto.Detalles)
            {
                if (detalle.IdProducto <= 0)
                    throw new ArgumentException("El ID del producto debe ser válido.");

                if (detalle.MontoDescuento < 0)
                    throw new ArgumentException("El descuento no puede ser negativo.");
            }

            // OBTENER IDS DE PRODUCTOS

            var idsProductos = dto.Detalles
                .Select(d => d.IdProducto)
                .ToList();

            // VALIDAR EXISTENCIA DE LOS PRODUCTOS

            if (!await _repository.ExistenProductosAsync(idsProductos))
                throw new KeyNotFoundException("Uno o más productos no existen.");

            // BUSCAR PRECIOS VIGENTES

            var fechaActual = DateTime.Now;

            var preciosVigentes = await _repository.ObtenerPreciosVigentesAsync(
                idsProductos,
                dto.IdMoneda,
                fechaActual
            );

            // COMPROBAR QUE TODOS LOS PRODUCTOS TENGAN PRECIO VIGENTE

            if (preciosVigentes.Count != idsProductos.Count)
                throw new InvalidOperationException(
                    "Uno o más productos no tienen un precio vigente para la moneda seleccionada."
                );

            // BUSCAR ESTADO PENDIENTE

            var estadoPendiente = await _repository.ObtenerEstadoOrdenAsync("Pendiente");

            if (estadoPendiente == null)
                throw new InvalidOperationException("No se encontró el estado Pendiente.");

            // CONSTRUIR DICCIONARIO DE PRECIOS

            var preciosPorProducto = preciosVigentes.ToDictionary(
                p => p.id_producto,
                p => p.precio_monto
            );

            // VALIDAR DESCUENTOS Y CONSTRUIR LA ORDEN

            var orden = new Ordenes_Compra
            {
                id_usuario = dto.IdUsuario,
                id_metodo_pago = dto.IdMetodoPago,
                id_moneda = dto.IdMoneda,
                id_estado_orden = estadoPendiente.id_estado_orden,

                Detalle_Ordens = dto.Detalles.Select(d =>
                {
                    var precioActual = preciosPorProducto[d.IdProducto];

                    if (d.MontoDescuento > precioActual)
                        throw new ArgumentException(
                            "El descuento no puede superar el precio del producto."
                        );

                    return new Detalle_Orden
                    {
                        id_producto = d.IdProducto,
                        precio_unitario = precioActual,
                        monto_descuento = d.MontoDescuento,
                        es_regalo = d.EsRegalo
                    };

                }).ToList()
            };

            // GUARDAR ORDEN

            var ordenCreada = await _repository.CrearAsync(orden);

            return MapearOrden(ordenCreada);
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

        // =========================================
        // ACTUALIZAR ESTADO DE ORDEN
        // =========================================

        public async Task<OrdenCompraDTO> ActualizarAsync(
            int id,
            OrdenCompraActualizarDTO dto)
        {
            // VALIDAR ID

            if (id <= 0)
                throw new ArgumentException("El ID de la orden debe ser mayor a cero.");

            // VALIDAR DTO

            if (dto == null || string.IsNullOrWhiteSpace(dto.EstadoOrden))
                throw new ArgumentException("El nuevo estado es obligatorio.");

            var nuevoEstadoNombre = dto.EstadoOrden.Trim();

            // VALIDAR ESTADO PERMITIDO

            if (nuevoEstadoNombre != "Completada" &&
                nuevoEstadoNombre != "Cancelada")
            {
                throw new ArgumentException(
                    "El estado debe ser Completada o Cancelada."
                );
            }

            // BUSCAR ORDEN

            var orden = await _repository.ObtenerPorIdParaActualizarAsync(id);

            if (orden == null)
                throw new KeyNotFoundException("No se encontró la orden de compra.");

            // VALIDAR ESTADO ACTUAL

            if (orden.id_estado_ordenNavigation.nombre != "Pendiente")
            {
                throw new InvalidOperationException(
                    "Solamente se pueden modificar órdenes que estén Pendientes."
                );
            }

            // BUSCAR NUEVO ESTADO EN LA BASE DE DATOS

            var nuevoEstado = await _repository.ObtenerEstadoOrdenAsync(
                nuevoEstadoNombre
            );

            if (nuevoEstado == null)
                throw new InvalidOperationException(
                    "No se encontró el estado indicado en la base de datos."
                );

            // MODIFICAR ESTADO

            orden.id_estado_orden = nuevoEstado.id_estado_orden;

            // GUARDAR CAMBIOS

            await _repository.ActualizarAsync(orden);

            // RECUPERAR ORDEN COMPLETA PARA DEVOLVER EL DTO

            var ordenActualizada = await _repository.ObtenerPorIdAsync(id);

            if (ordenActualizada == null)
                throw new KeyNotFoundException("No se pudo recuperar la orden actualizada.");

            return MapearOrden(ordenActualizada);
        }

    }
}
