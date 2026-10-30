using Microsoft.EntityFrameworkCore;
using PlayzoneApi.Models;
using PlayzoneApi.Repositories.Interfaces;

namespace PlayzoneApi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly PlayZoneContext _context;
        public UsuarioRepository(PlayZoneContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorEmailONombreUsuarioAsync(string identificador)
        {
            return await _context.Usuarios
                .Include(u => u.IdEstadoCuentaNavigation)
                .Include(u => u.IdPaisNavigation)
                .Include(u => u.NivelesUsuario)
                .FirstOrDefaultAsync(u => u.Email == identificador || u.NombreUsuario == identificador);
        }
    }
}
