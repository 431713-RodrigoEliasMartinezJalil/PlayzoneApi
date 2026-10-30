using PlayzoneApi.Models;

namespace PlayzoneApi.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmailONombreUsuarioAsync(string identificador);
    }
}
