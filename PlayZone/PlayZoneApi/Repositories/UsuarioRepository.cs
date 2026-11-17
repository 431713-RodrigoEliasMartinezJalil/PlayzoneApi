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

    }
}
