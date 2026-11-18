using System.ComponentModel.DataAnnotations;

namespace PlayZoneApi.DTOs.ProductoDTTOs
{
    public class ProductoActualizarDTO
    {
        public int? IdProductoPadre { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID del tipo de producto debe ser mayor a 0.")]
        public int? IdTipoProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID del estado de publicación debe ser mayor a 0.")]
        public int? IdEstadoPublicacion { get; set; }

        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string? Titulo { get; set; }
        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]

        public string? Descripcion { get; set; }

        [RegularExpression("^(12\\+|16\\+|18\\+|3\\+|7\\+|Libre)$", ErrorMessage = "La clasificación debe ser: 3+, 7+, 12+, 16+, 18+ o Libre.")]
        public string? ClasificacionEdad { get; set; }

        public DateOnly? FechaLanzamiento { get; set; }
    }
}
