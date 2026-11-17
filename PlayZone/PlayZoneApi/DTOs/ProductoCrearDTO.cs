
using System.ComponentModel.DataAnnotations;

namespace PlayZoneApi.DTOs
{
    public class ProductoCrearDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto padre debe ser mayor a 0.")]
        public int? IdProductoPadre { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de producto válido.")]
        public int IdTipoProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado de publicación válido.")]
        public int IdEstadoPublicacion { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La clasificación por edad es obligatoria.")]
        [RegularExpression("^(12\\+|16\\+|18\\+|3\\+|7\\+|Libre)$",
            ErrorMessage = "La clasificación debe ser: 3+, 7+, 12+, 16+, 18+ o Libre.")]
        public string ClasificacionEdad { get; set; } = string.Empty;

        public DateOnly FechaLanzamiento { get; set; }
    }
}
