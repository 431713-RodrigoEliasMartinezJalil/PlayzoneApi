using Microsoft.EntityFrameworkCore;
using PlayzoneApi.Models;

namespace PlayzoneApi.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly PlayZoneContext _context;

        public VentaRepository(PlayZoneContext context)
        {
            _context = context;
        }

        public async Task<bool> UsuarioExisteYEstaActivoAsync(int idUsuario)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.IdUsuario == idUsuario && u.IdEstadoCuentaNavigation.Estado == "Activa");
        }

        public async Task<List<int>> ObtenerProductosEnBibliotecaAsync(int idUsuario, IEnumerable<int> idsProductos)
        {
            return await _context.BibliotecaUsuarios
                .Where(b => b.IdUsuario == idUsuario && idsProductos.Contains(b.IdProducto))
                .Select(b => b.IdProducto)
                .ToListAsync();
        }

        public async Task<decimal?> ObtenerPrecioVigenteAsync(int idProducto, int idMoneda)
        {
            return await _context.PreciosProductos
                .Where(p => p.IdProducto == idProducto && p.IdMoneda == idMoneda && p.FechaFinVigencia == null)
                .Select(p => (decimal?)p.PrecioMonto)
                .FirstOrDefaultAsync();
        }

        public async Task<OrdenesCompra> ProcesarVentaTransaccionalAsync(
            OrdenesCompra orden,
            List<DetalleOrden> detalles,
            List<BibliotecaUsuario> bibliotecaItems)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.OrdenesCompras.Add(orden);
                await _context.SaveChangesAsync();

                foreach (var detalle in detalles)
                {
                    detalle.IdOrden = orden.IdOrden;
                    _context.DetalleOrdens.Add(detalle);
                }

                if (bibliotecaItems.Any())
                {
                    _context.BibliotecaUsuarios.AddRange(bibliotecaItems);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return orden;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrdenesCompra?> ObtenerVentaPorIdAsync(int idOrden)
        {
            return await _context.OrdenesCompras
                .Include(o => o.IdEstadoOrdenNavigation)
                .Include(o => o.IdMetodoPagoNavigation)
                .Include(o => o.IdMonedaNavigation)
                .Include(o => o.DetalleOrdens)
                    .ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(o => o.IdOrden == idOrden);
        }
    }
}
