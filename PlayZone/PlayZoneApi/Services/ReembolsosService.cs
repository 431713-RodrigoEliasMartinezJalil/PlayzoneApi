using PlayZoneApi.DTOs.ReembolsoDTOs;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;
using PlayZoneApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PlayZoneApi.Services
{
    public class ReembolsosService : IReembolsosService
    {
        private readonly IReembolsosRepository _repository;
        private readonly IOrdenesCompraRepository _ordenesRepository;

        public ReembolsosService(
            IReembolsosRepository repository,
            IOrdenesCompraRepository ordenesRepository)
        {
            _repository = repository;
            _ordenesRepository = ordenesRepository;
        }

        // =========================================
        // CREAR SOLICITUD DE REEMBOLSO
        // =========================================

        public async Task<ReembolsoDTO> CrearAsync(ReembolsoCrearDTO dto)
        {
            if (dto == null)
                throw new ArgumentException("Los datos del reembolso son obligatorios.");

            if (dto.IdDetalleOrden <= 0)
                throw new ArgumentException("El ID del detalle de orden debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(dto.Motivo))
                throw new ArgumentException("El motivo del reembolso es obligatorio.");

            var detalle = await _repository.ObtenerDetalleOrdenAsync(dto.IdDetalleOrden);

            if (detalle == null)
                throw new KeyNotFoundException("No se encontró el detalle de la orden.");

            if (detalle.id_ordenNavigation.id_estado_ordenNavigation.nombre != "Completada")
                throw new InvalidOperationException("Solamente se pueden solicitar reembolsos de órdenes completadas.");

            var existeReembolso = await _repository.ExisteReembolsoAsync(dto.IdDetalleOrden);

            if (existeReembolso)
                throw new InvalidOperationException("Este producto ya tiene una solicitud de reembolso.");

            var montoReembolsado = detalle.precio_unitario - detalle.monto_descuento;

            if (montoReembolsado <= 0)
                throw new InvalidOperationException("El monto a reembolsar debe ser mayor a cero.");

            var estado = await _repository.ObtenerEstadoReembolsoAsync("Solicitado");

            if (estado == null)
                throw new InvalidOperationException("No se encontró el estado Solicitado en la base de datos.");

            var reembolso = new Reembolso
            {
                id_detalle_orden = detalle.id_detalle_orden,
                id_estado_reembolso = estado.id_estado_reembolso,
                fecha_solicitud = DateTime.Now,
                motivo = dto.Motivo.Trim(),
                monto_reembolsado = montoReembolsado
            };

            var reembolsoCreado = await _repository.CrearAsync(reembolso);

            return new ReembolsoDTO
            {
                IdReembolso = reembolsoCreado.id_reembolso,
                IdDetalleOrden = detalle.id_detalle_orden,
                NombreProducto = detalle.id_productoNavigation.titulo,
                EstadoReembolso = estado.nombre,
                FechaSolicitud = reembolsoCreado.fecha_solicitud,
                Motivo = reembolsoCreado.motivo,
                MontoReembolsado = reembolsoCreado.monto_reembolsado
            };
        }

        // =========================================
        // OBTENER TODOS LOS REEMBOLSOS
        // =========================================

        public async Task<List<ReembolsoDTO>> ObtenerTodosAsync()
        {
            var reembolsos = await _repository.ObtenerTodosAsync();

            var respuesta = new List<ReembolsoDTO>();

            foreach (var reembolso in reembolsos)
            {
                var reembolsoDTO = new ReembolsoDTO
                {
                    IdReembolso = reembolso.id_reembolso,
                    IdDetalleOrden = reembolso.id_detalle_orden,
                    NombreProducto = reembolso.id_detalle_ordenNavigation.id_productoNavigation.titulo,
                    EstadoReembolso = reembolso.id_estado_reembolsoNavigation.nombre,
                    FechaSolicitud = reembolso.fecha_solicitud,
                    Motivo = reembolso.motivo,
                    MontoReembolsado = reembolso.monto_reembolsado
                };

                respuesta.Add(reembolsoDTO);
            }

            return respuesta;
        }

        // =========================================
        // OBTENER REEMBOLSO POR ID
        // =========================================

        public async Task<ReembolsoDTO> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del reembolso debe ser mayor a cero.");

            var reembolso = await _repository.ObtenerPorIdAsync(id);

            if (reembolso == null)
                throw new KeyNotFoundException("No se encontró el reembolso.");

            return new ReembolsoDTO
            {
                IdReembolso = reembolso.id_reembolso,
                IdDetalleOrden = reembolso.id_detalle_orden,
                NombreProducto = reembolso.id_detalle_ordenNavigation.id_productoNavigation.titulo,
                EstadoReembolso = reembolso.id_estado_reembolsoNavigation.nombre,
                FechaSolicitud = reembolso.fecha_solicitud,
                Motivo = reembolso.motivo,
                MontoReembolsado = reembolso.monto_reembolsado
            };
        }

        // =========================================
        // ACTUALIZAR ESTADO DEL REEMBOLSO
        // =========================================
        public async Task<ReembolsoDTO> ActualizarAsync(int id, ReembolsoActualizarDTO dto)
        {
            // 1. Buscamos el reembolso
            var reembolso = await _repository.ObtenerPorIdParaActualizarAsync(id);

            if (reembolso == null)
                throw new KeyNotFoundException("No se encontró el reembolso.");

            // 2. Buscamos el nuevo estado
            var nuevoEstado = await _repository.ObtenerEstadoReembolsoAsync(dto.Estado);

            if (nuevoEstado == null)
                throw new ArgumentException("El estado indicado no existe.");

            // 3. Validamos las transiciones permitidas
            string estadoActual = reembolso.id_estado_reembolsoNavigation.nombre;

            bool transicionValida =
                (estadoActual == "Solicitado" && dto.Estado == "Pendiente") ||
                (estadoActual == "Pendiente" &&
                    (dto.Estado == "Aprobado" || dto.Estado == "Rechazado"));

            if (!transicionValida)
                throw new InvalidOperationException(
                    "No se permite realizar esa transición de estado.");

            // 4. Actualizamos el estado del reembolso
            reembolso.id_estado_reembolso = nuevoEstado.id_estado_reembolso;

            // Guardamos la orden solamente si se cumplen las condiciones
            Ordenes_Compra? orden = null;

            if (dto.Estado == "Aprobado")
            {
                // 5. Obtenemos todos los detalles de la orden
                int idOrden = reembolso.id_detalle_ordenNavigation.id_orden;

                var detalles = await _repository.ObtenerDetallesPorOrdenAsync(idOrden);

                // 6. Verificamos si todos tienen un reembolso aprobado
                bool todosAprobados =
                    detalles.Count > 0 &&
                    detalles.All(d =>
                        d.Reembolso != null &&
                        (
                            d.Reembolso.id_reembolso == reembolso.id_reembolso ||
                            d.Reembolso.id_estado_reembolsoNavigation.nombre == "Aprobado"
                        )
                    );

                // 7. Si todos están aprobados, cancelamos la orden
                if (todosAprobados)
                {
                    orden = await _ordenesRepository.ObtenerPorIdParaActualizarAsync(idOrden);

                    if (orden == null)
                        throw new KeyNotFoundException("No se encontró la orden de compra.");

                    var estadoCancelada =
                        await _ordenesRepository.ObtenerEstadoOrdenAsync("Cancelada");

                    if (estadoCancelada == null)
                        throw new InvalidOperationException(
                            "No existe el estado Cancelada.");

                    orden.id_estado_orden = estadoCancelada.id_estado_orden;
                }
            }

            // 8. Guardamos ambos cambios en una misma transacción
            await _repository.ActualizarReembolsoYOrdenAsync(reembolso, orden);

            // 9. Devolvemos el reembolso actualizado
            return await ObtenerPorIdAsync(id);
        }

    }
}
