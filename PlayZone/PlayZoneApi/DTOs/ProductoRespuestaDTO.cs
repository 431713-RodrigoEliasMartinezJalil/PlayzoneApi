namespace PlayZoneApi.DTOs
{
    public class ProductoRespuestaDTO
    {
        public int IdProducto { get; set; }

        public int? IdProductoPadre { get; set; }

        public int IdTipoProducto { get; set; }

        public string TipoProducto { get; set; } = string.Empty;

        public int IdEstadoPublicacion { get; set; }

        public string EstadoPublicacion { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string ClasificacionEdad { get; set; } = string.Empty;

        public DateOnly FechaLanzamiento { get; set; }
    }
}