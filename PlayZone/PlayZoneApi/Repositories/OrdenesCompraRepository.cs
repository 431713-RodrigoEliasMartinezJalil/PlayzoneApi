
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

        public async Task<List<Ordenes_Compra>> ObtenerPorUsuarioAsync(int idUsuario)
        {
            var ordenes = await _context.Ordenes_Compras
                .AsNoTracking()
                .Include(o => o.id_usuarioNavigation)
                .Include(o => o.id_metodo_pagoNavigation)
                .Include(o => o.id_monedaNavigation)
                .Include(o => o.id_estado_ordenNavigation)
                .Include(o => o.Detalle_Ordens)
                    .ThenInclude(d => d.id_productoNavigation)
                .Where(o => o.id_usuario == idUsuario)
                .ToListAsync();

            return ordenes;
        }
    }
}
