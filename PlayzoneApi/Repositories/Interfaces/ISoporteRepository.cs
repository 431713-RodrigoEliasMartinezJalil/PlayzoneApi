using PlayzoneApi.Models;

namespace PlayzoneApi.Repositories.Interfaces
{
    public interface ISoporteRepository
    {
        Task<DetalleOrden?> ObtenerDetalleOrdenConProductoAsync(int idDetalleOrden, int idUsuario);
        Task<bool> ExisteReembolsoParaDetalleAsync(int idDetalleOrden);
        Task<Reembolso> RegistrarSolicitudReembolsoAsync(Reembolso reembolso);
        Task<bool> UsuarioPoseeProductoEnBibliotecaAsync(int idUsuario, int idProducto);
        Task<Reseña?> ObtenerReseñaExistenteAsync(int idUsuario, int idProducto);
        Task<Reseña> PublicarReseñaAsync(Reseña reseña);
    }
}
