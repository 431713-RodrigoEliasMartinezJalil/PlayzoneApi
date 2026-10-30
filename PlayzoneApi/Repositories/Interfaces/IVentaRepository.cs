using PlayzoneApi.Models;

namespace PlayzoneApi.Repositories.Interfaces
{
    public interface IVentaRepository
    {
        Task<bool> UsuarioExisteYEstaActivoAsync(int idUsuario);
        Task<List<int>> ObtenerProductosEnBibliotecaAsync(int idUsuario, IEnumerable<int> idsProductos);
        Task<decimal?> ObtenerPrecioVigenteAsync(int idProducto, int idMoneda);
        Task<OrdenesCompra> ProcesarVentaTransaccionalAsync(OrdenesCompra orden, List<DetalleOrden> detalles, List<BibliotecaUsuario> bibliotecaItems);
        Task<OrdenesCompra?> ObtenerVentaPorIdAsync(int idOrden);
    }
}
