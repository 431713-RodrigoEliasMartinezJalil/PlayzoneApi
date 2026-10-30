namespace PlayzoneApi.DTOs
{
    public class DetalleItemRequestDto
    {
        public int IdProducto { get; set; }
        public decimal MontoDescuento { get; set; } = 0;
        public bool EsRegalo { get; set; } = false;
    }
    public class CrearVentaRequestDto
    {
        public int IdUsuario { get; set; }
        public int IdMetodoPago { get; set; }
        public int IdMoneda { get; set; }
        public List<DetalleItemRequestDto> Items { get; set; } = new();
    }
    public class VentaRespuestaDto
    {
        public int IdOrden { get; set; }
        public int IdUsuario { get; set; }
        public string MetodoPago { get; set; } = null!;
        public string Moneda { get; set; } = null!;
        public string EstadoOrden { get; set; } = null!;
        public DateTime FechaOrden { get; set; }
        public decimal TotalBruto { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal TotalNeto => TotalBruto - TotalDescuentos;
        public List<DetalleItemRespuestaDto> Detalles { get; set; } = new();
    }
    public class DetalleItemRespuestaDto
    {
        public int IdDetalleOrden { get; set; }
        public int IdProducto { get; set; }
        public string TituloProducto { get; set; } = null!;
        public decimal PrecioUnitario { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal Subtotal => PrecioUnitario - MontoDescuento;
        public bool EsRegalo { get; set; }
    }
}
