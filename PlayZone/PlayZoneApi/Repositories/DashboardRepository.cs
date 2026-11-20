using Microsoft.EntityFrameworkCore;
using PlayZoneApi.DTOs.DashboardDTOs;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;
using System.Globalization;

namespace PlayZoneApi.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly PlayZoneContext _context;

        public DashboardRepository(PlayZoneContext context)
        {
            _context = context;
        }


        public async Task<List<IngresoMensualDTO>> ObtenerIngresosMensualesAsync(int anio)
        {
            // OBTENER INGRESOS DE ORDENES COMPLETADAS
            var resultado = await _context.Ordenes_Compras
                .Where(o =>
                    o.id_estado_ordenNavigation.nombre == "Completada" &&
                    o.fecha_orden.Year == anio)
                .SelectMany(o => o.Detalle_Ordens)
                .GroupBy(d => new
                {
                    Anio = d.id_ordenNavigation.fecha_orden.Year,
                    Mes = d.id_ordenNavigation.fecha_orden.Month,
                    Moneda = d.id_ordenNavigation.id_monedaNavigation.nombre,
                    CodigoMoneda = d.id_ordenNavigation.id_monedaNavigation.codigo
                })
                .Select(g => new
                {
                    g.Key.Anio,
                    g.Key.Mes,
                    g.Key.Moneda,
                    g.Key.CodigoMoneda,

                    CantidadOrdenes = g.Select(d => d.id_orden)
                                       .Distinct()
                                       .Count(),

                    JuegosVendidos = g.Count(),

                    MontoBruto = g.Sum(d => d.precio_unitario),

                    Descuentos = g.Sum(d => d.monto_descuento),

                    IngresoNeto = g.Sum(d =>
                        d.precio_unitario - d.monto_descuento)
                })
                .ToListAsync();

            // CONVERTIR RESULTADO AL DTO
            return resultado
                .Select(x => new IngresoMensualDTO
                {
                    Anio = x.Anio,
                    Mes = x.Mes,

                    NombreMes = CultureInfo
                        .GetCultureInfo("es-AR")
                        .DateTimeFormat
                        .GetMonthName(x.Mes),

                    Moneda = x.Moneda,
                    CodigoMoneda = x.CodigoMoneda,

                    CantidadOrdenes = x.CantidadOrdenes,
                    JuegosVendidos = x.JuegosVendidos,

                    MontoBruto = x.MontoBruto,
                    Descuentos = x.Descuentos,
                    IngresoNeto = x.IngresoNeto
                })
                .OrderBy(x => x.Mes)
                .ThenBy(x => x.CodigoMoneda)
                .ToList();
        }


        public async Task<DashboardResumenDTO> ObtenerResumenAsync()
        {
            // PRODUCTOS PUBLICADOS
            var productosPublicados = await _context.Productos
                .CountAsync(p => p.id_estado_publicacionNavigation.nombre == "Publicado");

            // TOTAL DE ORDENES
            var totalOrdenes = await _context.Ordenes_Compras
                .CountAsync();

            // ORDENES PENDIENTES
            var ordenesPendientes = await _context.Ordenes_Compras
                .CountAsync(o => o.id_estado_ordenNavigation.nombre == "Pendiente");

            // ORDENES COMPLETADAS
            var ordenesCompletadas = await _context.Ordenes_Compras
                .CountAsync(o => o.id_estado_ordenNavigation.nombre == "Completada");

            // REEMBOLSOS PENDIENTES DE RESOLUCION
            var reembolsosPendientes = await _context.Reembolsos
                .CountAsync(r =>
                    r.id_estado_reembolsoNavigation.nombre == "Solicitado" ||
                    r.id_estado_reembolsoNavigation.nombre == "Pendiente");

            // TOTAL DE USUARIOS
            var totalUsuarios = await _context.Usuarios
                .CountAsync();

            // TOTAL DE REEMBOLSOS
            var totalReembolsos = await _context.Reembolsos
                .CountAsync();

            // VENTAS AGRUPADAS POR MONEDA
            var ventasPorMoneda = await _context.Ordenes_Compras
                .Where(o => o.id_estado_ordenNavigation.nombre == "Completada")
                .SelectMany(o => o.Detalle_Ordens)
                .GroupBy(d => new
                {
                    d.id_ordenNavigation.id_monedaNavigation.nombre,
                    d.id_ordenNavigation.id_monedaNavigation.codigo
                })
                .Select(g => new VentasPorMonedaDTO
                {
                    Moneda = g.Key.nombre,
                    CodigoMoneda = g.Key.codigo,
                    TotalVendido = g.Sum(d =>
                        d.precio_unitario - d.monto_descuento)
                })
                .ToListAsync();

            // CONSTRUIR RESPUESTA
            return new DashboardResumenDTO
            {
                ProductosPublicados = productosPublicados,
                TotalOrdenes = totalOrdenes,
                OrdenesPendientes = ordenesPendientes,
                OrdenesCompletadas = ordenesCompletadas,
                ReembolsosPendientes = reembolsosPendientes,
                TotalUsuarios = totalUsuarios,
                TotalReembolsos = totalReembolsos,
                VentasPorMoneda = ventasPorMoneda
            };
        }


        public async Task<List<VideojuegoMasVendidoDTO>> ObtenerVideojuegosMasVendidosAsync(int limite)
        {
            // OBTENER VIDEOJUEGOS DE ORDENES COMPLETADAS
            var videojuegos = await _context.Detalle_Ordens
                .Where(d =>
                    d.id_ordenNavigation.id_estado_ordenNavigation.nombre == "Completada")
                .GroupBy(d => new
                {
                    d.id_producto,
                    d.id_productoNavigation.titulo
                })
                .Select(g => new VideojuegoMasVendidoDTO
                {
                    IdProducto = g.Key.id_producto,
                    Titulo = g.Key.titulo,

                    // CANTIDAD DE VENTAS
                    CantidadVentas = g.Count(),

                    // INGRESOS DESPUES DE DESCUENTOS
                    IngresosGenerados = g.Sum(d =>
                        d.precio_unitario - d.monto_descuento)
                })
                .OrderByDescending(v => v.CantidadVentas)
                .ThenBy(v => v.Titulo)
                .Take(limite)
                .ToListAsync();

            return videojuegos;
        }

        public async Task<List<ClienteMayorGastoDTO>> ObtenerClientesMayorGastoAsync(int limite)
        {
            // PRIMERA ETAPA: CALCULAR EL TOTAL DE CADA ORDEN
            var comprasPorOrden = _context.Detalle_Ordens
                .Where(d =>
                    d.id_ordenNavigation.id_estado_ordenNavigation.nombre == "Completada")
                .GroupBy(d => new
                {
                    d.id_orden,
                    d.id_ordenNavigation.id_usuario,
                    d.id_ordenNavigation.id_usuarioNavigation.nombre_usuario,
                    d.id_ordenNavigation.id_usuarioNavigation.nombre,
                    d.id_ordenNavigation.id_usuarioNavigation.apellido,
                    d.id_ordenNavigation.id_moneda
                })
                .Select(g => new
                {
                    g.Key.id_usuario,
                    g.Key.nombre_usuario,
                    g.Key.nombre,
                    g.Key.apellido,
                    g.Key.id_moneda,

                    TotalOrden = g.Sum(d =>
                        d.precio_unitario - d.monto_descuento)
                });

            // SEGUNDA ETAPA: AGRUPAR LAS ORDENES POR CLIENTE Y MONEDA
            var clientes = await comprasPorOrden
                .GroupBy(c => new
                {
                    c.id_usuario,
                    c.nombre_usuario,
                    c.nombre,
                    c.apellido,
                    c.id_moneda
                })
                .Select(g => new ClienteMayorGastoDTO
                {
                    IdUsuario = g.Key.id_usuario,
                    NombreUsuario = g.Key.nombre_usuario,
                    Nombre = g.Key.nombre,
                    Apellido = g.Key.apellido,
                    IdMoneda = g.Key.id_moneda,

                    CantidadCompras = g.Count(),

                    TotalGastado = g.Sum(c => c.TotalOrden),

                    TicketPromedio = g.Average(c => c.TotalOrden)
                })
                .OrderByDescending(c => c.TotalGastado)
                .Take(limite)
                .ToListAsync();

            return clientes;
        }
    }
}