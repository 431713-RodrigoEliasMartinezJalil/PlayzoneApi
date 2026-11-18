using Microsoft.EntityFrameworkCore;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;

namespace PlayZoneApi.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly PlayZoneContext _context;

        public DashboardRepository(PlayZoneContext context)
        {
            _context = context;
        }
        public async Task<int> ObtenerTotalOrdenesAsync()
        {
            return await _context.Ordenes_Compras
                .CountAsync();
        }

        // ORDENES SEGUN SU ESTADO
        public async Task<int> ObtenerOrdenesPorEstadoAsync(string estado)
        {
            return await _context.Ordenes_Compras
                .CountAsync(o =>
                    o.id_estado_ordenNavigation.nombre == estado);
        }

        // TOTAL DE PRODUCTOS VISIBLES
        public async Task<int> ObtenerTotalProductosAsync()
        {
            return await _context.Productos
                .CountAsync(p => p.id_estado_publicacion != 3);
        }

        // TOTAL DE USUARIOS
        public async Task<int> ObtenerTotalUsuariosAsync()
        {
            return await _context.Usuarios
                .CountAsync();
        }

        // TOTAL DE REEMBOLSOS
        public async Task<int> ObtenerTotalReembolsosAsync()
        {
            return await _context.Reembolsos
                .CountAsync();
        }
    }
}

