namespace PlayzoneApi.DTOs
{
    public class ProductoFiltroDto
    {
        public string? Busqueda { get; set; }
        public int? IdTipoProducto { get; set; }
        public int? IdGenero { get; set; }
        public int? IdEtiqueta { get; set; }
        public int IdMoneda { get; set; } = 1;
        public string CampoOrden { get; set; } = "titulo";
        public bool EsDescendente { get; set; } = false;
    }

    public class ProductoResumenDto
    {
        public int IdProducto { get; set; }
        public string Titulo { get; set; } = null!;
        public string ClasificacionEdad { get; set; } = null!;
        public DateOnly FechaLanzamiento { get; set; }
        public string TipoProducto { get; set; } = null!;
        public decimal PrecioMonto { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal PrecioFinal => PrecioMonto * (1 - (PorcentajeDescuento / 100));
        public List<string> Generos { get; set; } = new();
    }

    public class ProductoDetalleDto : ProductoResumenDto
    {
        public string? Descripcion { get; set; }
        public string EstadoPublicacion { get; set; } = null!;
        public List<string> Empresas { get; set; } = new();
        public List<string> Etiquetas { get; set; } = new();
        public List<ProductoResumenDto> ContenidoAdicional { get; set; } = new();
    }
}
