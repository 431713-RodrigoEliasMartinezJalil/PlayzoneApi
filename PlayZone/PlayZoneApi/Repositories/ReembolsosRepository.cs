
using Microsoft.EntityFrameworkCore;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;

namespace PlayZoneApi.Repositories
{
    public class ReembolsosRepository : IReembolsosRepository
    {
        private readonly PlayZoneContext _context;

        public ReembolsosRepository(PlayZoneContext context)
        {
            _context = context;
        }

        // =========================================
        // OBTENER DETALLE DE ORDEN
        // =========================================


        public async Task<Detalle_Orden?> ObtenerDetalleOrdenAsync(int idDetalleOrden)
        {
            return await _context.Detalle_Ordens
                .AsNoTracking()
                .Include(d => d.id_ordenNavigation)
                    .ThenInclude(o => o.id_estado_ordenNavigation)
                .Include(d => d.id_productoNavigation)
                .FirstOrDefaultAsync(d => d.id_detalle_orden == idDetalleOrden);
        }


        // =========================================
        // VERIFICAR SI YA EXISTE UN REEMBOLSO
        // =========================================

        public async Task<bool> ExisteReembolsoAsync(int idDetalleOrden)
        {
            return await _context.Set<Reembolso>()
                .AnyAsync(r => r.id_detalle_orden == idDetalleOrden);
        }

        // =========================================
        // OBTENER ESTADO DE REEMBOLSO POR NOMBRE
        // =========================================

        public async Task<Estados_Reembolso?> ObtenerEstadoReembolsoAsync(string nombre)
        {
            return await _context.Set<Estados_Reembolso>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.nombre == nombre);
        }

        // =========================================
        // CREAR REEMBOLSO
        // =========================================

        public async Task<Reembolso> CrearAsync(Reembolso reembolso)
        {
            await _context.Set<Reembolso>().AddAsync(reembolso);

            await _context.SaveChangesAsync();

            return reembolso;
        }


        // =========================================
        // OBTENER TODOS LOS REEMBOLSOS
        // =========================================

        public async Task<List<Reembolso>> ObtenerTodosAsync()
        {
            return await _context.Set<Reembolso>()
                .AsNoTracking()
                .Include(r => r.id_estado_reembolsoNavigation)
                .Include(r => r.id_detalle_ordenNavigation)
                    .ThenInclude(d => d.id_productoNavigation)
                .ToListAsync();
        }

        // =========================================
        // OBTENER REEMBOLSOS POR ID
        // =========================================
        public async Task<Reembolso?> ObtenerPorIdAsync(int id)
        {
            return await _context.Set<Reembolso>()
                .AsNoTracking()
                .Include(r => r.id_estado_reembolsoNavigation)
                .Include(r => r.id_detalle_ordenNavigation)
                    .ThenInclude(d => d.id_productoNavigation)
                .FirstOrDefaultAsync(r => r.id_reembolso == id);
        }

        public async Task<Reembolso?> ObtenerPorIdParaActualizarAsync(int id)
        {
            return await _context.Set<Reembolso>()
                .Include(r => r.id_estado_reembolsoNavigation)
                .Include(r => r.id_detalle_ordenNavigation)
                .FirstOrDefaultAsync(r => r.id_reembolso == id);
        }

        public async Task<Reembolso> ActualizarAsync(Reembolso reembolso)
        {
            await _context.SaveChangesAsync();

            return reembolso;
        }

        public async Task<List<Detalle_Orden>> ObtenerDetallesPorOrdenAsync(int idOrden)
        {
            return await _context.Detalle_Ordens
                .Include(d => d.Reembolso)
                    .ThenInclude(r => r.id_estado_reembolsoNavigation)
                .Where(d => d.id_orden == idOrden)
                .ToListAsync();
        }

        public async Task ActualizarReembolsoYOrdenAsync(
    Reembolso reembolso,
    Ordenes_Compra? orden)
        {
            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.SaveChangesAsync();

                await transaccion.CommitAsync();
            }
            catch
            {
                await transaccion.RollbackAsync();
                throw;
            }
        }

    }
}
