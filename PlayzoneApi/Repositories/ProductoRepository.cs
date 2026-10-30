using Microsoft.EntityFrameworkCore;
using PlayzoneApi.DTOs;
using PlayzoneApi.Models;
using PlayzoneApi.Repositories.Interfaces;

namespace PlayzoneApi.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly PlayZoneContext _context;
        public ProductoRepository(PlayZoneContext context)
        {
            _context = context;
        }

        public async Task<ResultadoPaginadoDto<ProductoResumenDto>> ListarCatalogoPaginadoAsync(
            ProductoFiltroDto filtro,
            int pagina = 1,
            int tamanoPagina = 10)
        {
            var query = _context.Productos
                .Include(p => p.IdTipoProductoNavigation)
                .Include(p => p.PreciosProductos)
                .Include(p => p.ProductosGeneros)
                    .ThenInclude(pg => pg.IdGeneroNavigation)
                .Include(p => p.PromocionesProductos)
                    .ThenInclude(pp => pp.IdPromocionNavigation)
                .Where(p => p.IdEstadoPublicacionNavigation.Nombre == "Publicado")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
            {
                query = query.Where(p => p.Titulo.Contains(filtro.Busqueda));
            }

            if (filtro.IdTipoProducto.HasValue)
            {
                query = query.Where(p => p.IdTipoProducto == filtro.IdTipoProducto.Value);
            }

            if (filtro.IdGenero.HasValue)
            {
                query = query.Where(p => p.ProductosGeneros.Any(pg => pg.IdGenero == filtro.IdGenero.Value));
            }

            int totalRegistros = await query.CountAsync();

            query = AplicarOrdenamientoDinamico(query, filtro.CampoOrden, filtro.EsDescendente, filtro.IdMoneda);

            var productosPaginados = await query
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            var itemsDto = productosPaginados
                .Select(p => MapearAProductoResumenDto(p, filtro.IdMoneda))
                .ToList();

            return new ResultadoPaginadoDto<ProductoResumenDto>
            {
                Items = itemsDto,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<Producto?> ObtenerPorIdConRelacionesAsync(int idProducto)
        {
            return await _context.Productos
                .Include(p => p.IdTipoProductoNavigation)
                .Include(p => p.IdEstadoPublicacionNavigation)
                .Include(p => p.PreciosProductos)
                .Include(p => p.ProductosGeneros)
                    .ThenInclude(pg => pg.IdGeneroNavigation)
                .Include(p => p.ProductosEtiqueta)
                    .ThenInclude(pe => pe.IdEtiquetaNavigation)
                .Include(p => p.ProductosEmpresas)
                    .ThenInclude(pe => pe.IdEmpresaNavigation)
                .Include(p => p.PromocionesProductos)
                    .ThenInclude(pp => pp.IdPromocionNavigation)
                .Include(p => p.InverseIdProductoPadreNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);
        }

        private static IQueryable<Producto> AplicarOrdenamientoDinamico(
            IQueryable<Producto> query,
            string campoOrden,
            bool esDescendente,
            int idMoneda)
        {
            campoOrden = campoOrden.ToLower().Trim();

            return (campoOrden, esDescendente) switch
            {
                ("precio", false) => query.OrderBy(p => p.PreciosProductos
                    .Where(pr => pr.IdMoneda == idMoneda && pr.FechaFinVigencia == null)
                    .Select(pr => (decimal?)pr.PrecioMonto)
                    .FirstOrDefault() ?? 0),

                ("precio", true) => query.OrderByDescending(p => p.PreciosProductos
                    .Where(pr => pr.IdMoneda == idMoneda && pr.FechaFinVigencia == null)
                    .Select(pr => (decimal?)pr.PrecioMonto)
                    .FirstOrDefault() ?? 0),

                ("fecha", false) => query.OrderBy(p => p.FechaLanzamiento),
                ("fecha", true) => query.OrderByDescending(p => p.FechaLanzamiento),

                (_, true) => query.OrderByDescending(p => p.Titulo),
                _ => query.OrderBy(p => p.Titulo)
            };
        }

        private static ProductoResumenDto MapearAProductoResumenDto(Producto p, int idMoneda)
        {
            var precio = p.PreciosProductos.FirstOrDefault(pr => pr.IdMoneda == idMoneda && pr.FechaFinVigencia == null);
            var promo = p.PromocionesProductos
                .Select(pp => pp.IdPromocionNavigation)
                .FirstOrDefault(pr => pr.Activa && pr.FechaInicio <= DateTime.UtcNow && pr.FechaFin >= DateTime.UtcNow);

            return new ProductoResumenDto
            {
                IdProducto = p.IdProducto,
                Titulo = p.Titulo,
                ClasificacionEdad = p.ClasificacionEdad,
                FechaLanzamiento = p.FechaLanzamiento,
                TipoProducto = p.IdTipoProductoNavigation?.Nombre ?? "",
                PrecioMonto = precio?.PrecioMonto ?? 0,
                PorcentajeDescuento = promo?.PorcentajeDescuento ?? 0,
                Generos = p.ProductosGeneros.Select(pg => pg.IdGeneroNavigation.Nombre).ToList()
            };
        }
    }
}
