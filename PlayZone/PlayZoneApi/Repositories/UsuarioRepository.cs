using Microsoft.EntityFrameworkCore;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;

namespace PlayZoneApi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly PlayZoneContext db;

        public UsuarioRepository(PlayZoneContext db)
        {
            this.db = db;
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            return await db.Usuarios.Include(u => u.id_estado_cuentaNavigation).FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario)
        {
            return await db.Usuarios.AnyAsync(u => u.nombre_usuario == nombreUsuario);
        }

        public async Task<Estado_Cuentum?> ObtenerEstadoPorNombreAsync(string estado)
        {
            return await db.Estado_Cuenta.FirstOrDefaultAsync(e => e.estado == estado);
        }

        public async Task<Pai?> ObtenerPaisPorIdAsync(int idPais)
        {
            return await db.Pais.FirstOrDefaultAsync(p => p.id_pais == idPais);
        }

        public async Task CrearAsync(Usuario usuario)
        {
            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();
        }

    }
}
