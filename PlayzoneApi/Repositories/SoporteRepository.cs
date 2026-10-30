using Microsoft.EntityFrameworkCore;
using PlayzoneApi.Models;

namespace PlayzoneApi.Repositories
{
    public class SoporteRepository : ISoporteRepository
    {
        private readonly PlayZoneContext _context;

        public SoporteRepository(PlayZoneContext context)
        {
            _context = context;
        }

        public async Task<DetalleOrden?> ObtenerDetalleOrdenConProductoAsync(int idDetalleOrden, int idUsuario)
        {
            return await _context.DetalleOrdens
                .Include(d => d.IdOrdenNavigation)
                .FirstOrDefaultAsync(d => d.IdDetalleOrden == idDetalleOrden && d.IdOrdenNavigation.IdUsuario == idUsuario);
        }

        public async Task<bool> ExisteReembolsoParaDetalleAsync(int idDetalleOrden)
        {
            return await _context.Reembolsos.AnyAsync(r => r.IdDetalleOrden == idDetalleOrden);
        }

        public async Task<Reembolso> RegistrarSolicitudReembolsoAsync(Reembolso reembolso)
        {
            _context.Reembolsos.Add(reembolso);
            await _context.SaveChangesAsync();
            return reembolso;
        }

        public async Task<bool> UsuarioPoseeProductoEnBibliotecaAsync(int idUsuario, int idProducto)
        {
            return await _context.BibliotecaUsuarios.AnyAsync(b => b.IdUsuario == idUsuario && b.IdProducto == idProducto);
        }

        public async Task<Reseña?> ObtenerReseñaExistenteAsync(int idUsuario, int idProducto)
        {
            return await _context.Reseñas.FirstOrDefaultAsync(r => r.IdUsuario == idUsuario && r.IdProducto == idProducto);
        }

        public async Task<Reseña> PublicarReseñaAsync(Reseña reseña)
        {
            _context.Reseñas.Add(reseña);
            await _context.SaveChangesAsync();
            return reseña;
        }
    }
}
