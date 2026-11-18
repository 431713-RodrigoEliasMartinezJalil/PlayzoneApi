
using Microsoft.EntityFrameworkCore;
using PlayZoneApi.DTOs.ProductoDTTOs;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;

namespace PlayZoneApi.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly PlayZoneContext _context;

        public ProductoRepository(PlayZoneContext context)
        {
            _context = context;
        }

        // LISTAR Y FILTRAR PRODUCTOS
        public async Task<List<Producto>> ObtenerTodosAsync(ProductoFiltroDTO filtros)
        {
            var consulta = _context.Productos
                .Include(p => p.id_tipo_productoNavigation)
                .Include(p => p.id_estado_publicacionNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtros.Titulo))
                consulta = consulta.Where(p => p.titulo.Contains(filtros.Titulo));

            if (filtros.IdTipoProducto.HasValue)
                consulta = consulta.Where(p => p.id_tipo_producto == filtros.IdTipoProducto.Value);

            if (filtros.IdEstadoPublicacion.HasValue)
                consulta = consulta.Where(p => p.id_estado_publicacion == filtros.IdEstadoPublicacion.Value);

            if (!string.IsNullOrWhiteSpace(filtros.ClasificacionEdad))
                consulta = consulta.Where(p => p.clasificacion_edad == filtros.ClasificacionEdad);

            return await consulta.AsNoTracking().ToListAsync();
        }

        // OBTENER PRODUCTO POR ID
        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.id_tipo_productoNavigation)
                .Include(p => p.id_estado_publicacionNavigation)
                .FirstOrDefaultAsync(p => p.id_producto == id);
        }

        // OBTENER TIPO DE PRODUCTO
        public async Task<Tipos_Producto?> ObtenerTipoProductoAsync(int id)
        {
            return await _context.Tipos_Productos
                .FirstOrDefaultAsync(t => t.id_tipo_producto == id);
        }

        // OBTENER ESTADO DE PUBLICACIÓN
        public async Task<Estados_Publicacion?> ObtenerEstadoPublicacionAsync(int id)
        {
            return await _context.Estados_Publicacions
                .FirstOrDefaultAsync(e => e.id_estado_publicacion == id);
        }

        // AGREGAR PRODUCTO
        public async Task AgregarAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
        }

        // ACTUALIZAR PRODUCTO
        public async Task ActualizarAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        // ELIMINAR PRODUCTO
        public async Task EliminarAsync(Producto producto)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }
}
