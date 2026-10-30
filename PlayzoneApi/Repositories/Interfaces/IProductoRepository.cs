using PlayzoneApi.DTOs;
using PlayzoneApi.Models;

namespace PlayzoneApi.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        Task<ResultadoPaginadoDto<ProductoResumenDto>> ListarCatalogoPaginadoAsync(ProductoFiltroDto filtro, int pagina, int tamanoPagina);
        Task<Producto?> ObtenerPorIdConRelacionesAsync(int idProducto);
    }
}
