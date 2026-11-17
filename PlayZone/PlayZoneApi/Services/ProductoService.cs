
using PlayZoneApi.DTOs;
using PlayZoneApi.Models;
using PlayZoneApi.Repositories.Interfaces;
using PlayZoneApi.Services.Interfaces;

namespace PlayZoneApi.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repository;

        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        // LISTAR Y FILTRAR PRODUCTOS
        public async Task<List<ProductoRespuestaDTO>> ObtenerTodosAsync(
            ProductoFiltroDTO filtros)
        {
            var productos = await _repository.ObtenerTodosAsync(filtros);

            return productos.Select(p => MapearARespuestaDTO(p)).ToList();
        }

        // OBTENER PRODUCTO POR ID
        public async Task<ProductoRespuestaDTO?> ObtenerPorIdAsync(int id)
        {
            var producto = await _repository.ObtenerPorIdAsync(id);

            if (producto == null)
                return null;

            return MapearARespuestaDTO(producto);
        }

        // AGREGAR PRODUCTO
        public async Task<int> AgregarAsync(ProductoCrearDTO dto)
        {
            await ValidarReferenciasAsync(
                dto.IdTipoProducto,
                dto.IdEstadoPublicacion,
                dto.IdProductoPadre
            );

            var producto = new Producto
            {
                id_producto_padre = dto.IdProductoPadre,
                id_tipo_producto = dto.IdTipoProducto,
                id_estado_publicacion = dto.IdEstadoPublicacion,
                titulo = dto.Titulo,
                descripcion = dto.Descripcion,
                clasificacion_edad = dto.ClasificacionEdad,
                fecha_lanzamiento = dto.FechaLanzamiento
            };

            await _repository.AgregarAsync(producto);

            return producto.id_producto;
        }

        // ACTUALIZAR PRODUCTO COMPLETO
        public async Task<bool> ActualizarAsync(int id, ProductoCrearDTO dto)
        {
            var producto = await _repository.ObtenerPorIdAsync(id);

            if (producto == null)
                return false;

            await ValidarReferenciasAsync(
                dto.IdTipoProducto,
                dto.IdEstadoPublicacion,
                dto.IdProductoPadre,
                id
            );

            producto.id_producto_padre = dto.IdProductoPadre;
            producto.id_tipo_producto = dto.IdTipoProducto;
            producto.id_estado_publicacion = dto.IdEstadoPublicacion;
            producto.titulo = dto.Titulo;
            producto.descripcion = dto.Descripcion;
            producto.clasificacion_edad = dto.ClasificacionEdad;
            producto.fecha_lanzamiento = dto.FechaLanzamiento;

            await _repository.ActualizarAsync(producto);

            return true;
        }

        // ACTUALIZAR PRODUCTO PARCIALMENTE
        public async Task<bool> ActualizarParcialAsync(
            int id, ProductoActualizarDTO dto)
        {
            var producto = await _repository.ObtenerPorIdAsync(id);

            if (producto == null)
                return false;

            await ValidarReferenciasAsync(
                dto.IdTipoProducto ?? producto.id_tipo_producto,
                dto.IdEstadoPublicacion ?? producto.id_estado_publicacion,
                dto.IdProductoPadre ?? producto.id_producto_padre,
                id
            );

            if (dto.IdProductoPadre.HasValue)
                producto.id_producto_padre = dto.IdProductoPadre.Value;

            if (dto.IdTipoProducto.HasValue)
                producto.id_tipo_producto = dto.IdTipoProducto.Value;

            if (dto.IdEstadoPublicacion.HasValue)
                producto.id_estado_publicacion = dto.IdEstadoPublicacion.Value;

            if (dto.Titulo != null)
                producto.titulo = dto.Titulo;

            if (dto.Descripcion != null)
                producto.descripcion = dto.Descripcion;

            if (dto.ClasificacionEdad != null)
                producto.clasificacion_edad = dto.ClasificacionEdad;

            if (dto.FechaLanzamiento.HasValue)
                producto.fecha_lanzamiento = dto.FechaLanzamiento.Value;

            await _repository.ActualizarAsync(producto);

            return true;
        }

        // ELIMINAR PRODUCTO
        public async Task<bool> EliminarAsync(int id)
        {
            var producto = await _repository.ObtenerPorIdAsync(id);

            if (producto == null)
                return false;

            await _repository.EliminarAsync(producto);

            return true;
        }

        // VALIDAR EXISTENCIA DE LAS REFERENCIAS
        private async Task ValidarReferenciasAsync(
            int idTipoProducto,
            int idEstadoPublicacion,
            int? idProductoPadre,
            int? idProductoActual = null)
        {
            var tipo = await _repository.ObtenerTipoProductoAsync(idTipoProducto);

            if (tipo == null)
                throw new ArgumentException("El tipo de producto indicado no existe.");

            var estado = await _repository.ObtenerEstadoPublicacionAsync(idEstadoPublicacion);

            if (estado == null)
                throw new ArgumentException("El estado de publicación indicado no existe.");

            if (idProductoPadre.HasValue)
            {
                if (idProductoActual.HasValue &&
                    idProductoPadre.Value == idProductoActual.Value)
                {
                    throw new ArgumentException(
                        "Un producto no puede ser su propio producto padre.");
                }

                var padre = await _repository.ObtenerPorIdAsync(idProductoPadre.Value);

                if (padre == null)
                    throw new ArgumentException("El producto padre indicado no existe.");
            }
        }

        // CONVERTIR ENTIDAD A DTO DE RESPUESTA
        private ProductoRespuestaDTO MapearARespuestaDTO(Producto producto)
        {
            return new ProductoRespuestaDTO
            {
                IdProducto = producto.id_producto,
                IdProductoPadre = producto.id_producto_padre,
                IdTipoProducto = producto.id_tipo_producto,
                TipoProducto = producto.id_tipo_productoNavigation?.nombre ?? "",
                IdEstadoPublicacion = producto.id_estado_publicacion,
                EstadoPublicacion = producto.id_estado_publicacionNavigation?.nombre ?? "",
                Titulo = producto.titulo,
                Descripcion = producto.descripcion,
                ClasificacionEdad = producto.clasificacion_edad,
                FechaLanzamiento = producto.fecha_lanzamiento
            };
        }
    }
}
