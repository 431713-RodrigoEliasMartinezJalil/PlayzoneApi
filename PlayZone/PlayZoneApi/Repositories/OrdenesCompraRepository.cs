
using Microsoft.EntityFrameworkCore;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;

namespace PlayZoneApi.Repositories
{
    public class OrdenesCompraRepository : IOrdenesCompraRepository
    {
        private readonly PlayZoneContext _context;

        public OrdenesCompraRepository(PlayZoneContext context)
        {
            _context = context;
        }

        // =========================================
        // OBTENER TODAS LAS ORDENES
        // =========================================

        public async Task<List<Ordenes_Compra>> ObtenerTodasAsync()
        {
            var ordenes = await _context.Ordenes_Compras
                .AsNoTracking()
                .Include(o => o.id_usuarioNavigation)
                .Include(o => o.id_metodo_pagoNavigation)
                .Include(o => o.id_monedaNavigation)
                .Include(o => o.id_estado_ordenNavigation)
                .Include(o => o.Detalle_Ordens)
                    .ThenInclude(d => d.id_productoNavigation)
                .ToListAsync();

            return ordenes;
        }

        // =========================================
        // OBTENER ORDEN POR ID
        // =========================================

        public async Task<Ordenes_Compra?> ObtenerPorIdAsync(int id)
        {
            var orden = await _context.Ordenes_Compras
                .AsNoTracking()
                .Include(o => o.id_usuarioNavigation)
                .Include(o => o.id_metodo_pagoNavigation)
                .Include(o => o.id_monedaNavigation)
                .Include(o => o.id_estado_ordenNavigation)
                .Include(o => o.Detalle_Ordens)
                    .ThenInclude(d => d.id_productoNavigation)
                .FirstOrDefaultAsync(o => o.id_orden == id);

            return orden;
        }

        // =========================================
        // OBTENER ORDENES POR USUARIO
        // =========================================

        public async Task<List<Ordenes_Compra>> ObtenerPorUsuarioAsync(string nombreUsuario)
        {
            var ordenes = await _context.Ordenes_Compras
                .AsNoTracking()
                .Include(o => o.id_usuarioNavigation)
                .Include(o => o.id_metodo_pagoNavigation)
                .Include(o => o.id_monedaNavigation)
                .Include(o => o.id_estado_ordenNavigation)
                .Include(o => o.Detalle_Ordens)
                    .ThenInclude(d => d.id_productoNavigation)
                .Where(o => o.id_usuarioNavigation.nombre_usuario == nombreUsuario)
                .ToListAsync();

            return ordenes;
        }

        // =========================================
        // CREAR ORDEN DE COMPRA
        // =========================================

        public async Task<Ordenes_Compra> CrearAsync(Ordenes_Compra orden)
        {
            // Iniciamos una transacción en la base de datos.
            await using var transaccion = await _context.Database.BeginTransactionAsync();

            try
            {
                // Agregamos la orden con todos sus detalles.
                await _context.Ordenes_Compras.AddAsync(orden);

                // Guardamos todo en la base de datos.
                await _context.SaveChangesAsync();

                // Confirmamos la transacción.
                await transaccion.CommitAsync();

                // Volvemos a consultar la orden para recuperar
                // también sus relaciones (usuario, estado, productos, etc.).
                var ordenGuardada = await ObtenerPorIdAsync(orden.id_orden);

                if (ordenGuardada == null)
                    throw new InvalidOperationException("No se pudo recuperar la orden creada.");

                return ordenGuardada;
            }
            catch
            {
                // Si algo falla, revertimos los cambios.
                await transaccion.RollbackAsync();

                throw;
            }
        }

        // =========================================
        // OBTENER ESTADO DE ORDEN POR NOMBRE
        // =========================================

        public async Task<Estados_Orden?> ObtenerEstadoOrdenAsync(string nombre)
        {
            return await _context.Estados_Ordens
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.nombre == nombre);
        }

        // =========================================
        // VERIFICAR EXISTENCIA DE USUARIO
        // =========================================

        public async Task<bool> ExisteUsuarioAsync(int id)
        {
            return await _context.Set<Usuario>()
                .AnyAsync(u => u.id_usuario == id);
        }

        // =========================================
        // VERIFICAR EXISTENCIA DE MÉTODO DE PAGO
        // =========================================

        public async Task<bool> ExisteMetodoPagoAsync(int id)
        {
            return await _context.Set<Metodos_Pago>()
                .AnyAsync(m => m.id_metodo_pago == id);
        }

        // =========================================
        // VERIFICAR EXISTENCIA DE MONEDA
        // =========================================

        public async Task<bool> ExisteMonedaAsync(int id)
        {
            return await _context.Set<Moneda>()
                .AnyAsync(m => m.id_moneda == id);
        }

        // =========================================
        // VERIFICAR EXISTENCIA DE PRODUCTOS
        // =========================================

        public async Task<bool> ExistenProductosAsync(List<int> ids)
        {
            var cantidadEncontrada = await _context.Set<Producto>()
                .CountAsync(p => ids.Contains(p.id_producto));

            return cantidadEncontrada == ids.Distinct().Count();
        }

        // =========================================
        // OBTENER PRECIOS VIGENTES
        // =========================================
        public async Task<List<Precios_Producto>> ObtenerPreciosVigentesAsync(
            List<int> idsProductos,
            int idMoneda,
            DateTime fecha)
        {
            return await _context.Set<Precios_Producto>()
                .AsNoTracking()
                .Where(p =>
                    idsProductos.Contains(p.id_producto) &&
                    p.id_moneda == idMoneda &&
                    p.fecha_inicio_vigencia <= fecha &&
                    (p.fecha_fin_vigencia == null ||
                     p.fecha_fin_vigencia > fecha))
                .ToListAsync();
        }

        // =========================================
        // OBTENER ORDEN PARA ACTUALIZAR
        // =========================================

        public async Task<Ordenes_Compra?> ObtenerPorIdParaActualizarAsync(int id)
        {
            return await _context.Ordenes_Compras
                .Include(o => o.id_estado_ordenNavigation)
                .FirstOrDefaultAsync(o => o.id_orden == id);
        }

        // =========================================
        // ACTUALIZAR ORDEN
        // =========================================

        public async Task ActualizarAsync(Ordenes_Compra orden)
        {
            await _context.SaveChangesAsync();
        }

        // =========================================
        // CREAR ORDEN DE COMPRA COMPLETO
        // =========================================

        public async Task<bool> CrearCompletoAsync(int id, Ordenes_Compra orden)
        {
            try
            {
                // Busco la orden para actualizar
                Ordenes_Compra? viejaOrden = await _context.Ordenes_Compras
                .Include(o => o.Detalle_Ordens)
                .FirstOrDefaultAsync(o => o.id_orden == id);
                // Valido que exista
                if (viejaOrden == null)
                {
                    throw new InvalidOperationException("La orden a actualizar no existe.");
                }

                // Actualizamos la orden
                viejaOrden.id_usuario = orden.id_usuario;
                viejaOrden.id_metodo_pago = orden.id_metodo_pago;
                viejaOrden.id_moneda = orden.id_moneda;
                _context.Detalle_Ordens.RemoveRange(viejaOrden.Detalle_Ordens);
                viejaOrden.Detalle_Ordens = orden.Detalle_Ordens;

                // Guardamos todo en la base de datos.
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                throw new KeyNotFoundException("Ocurrió un error al intentar actualizar.");
            }
        }
    }
}
