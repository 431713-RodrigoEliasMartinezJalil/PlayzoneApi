namespace PlayZoneApi.DTOs.OrdenDTOs
{
    public class DetalleOrdenDTO
    {
        public int IdProducto { get; set; }

        public string TituloProducto { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal MontoDescuento { get; set; }

        public bool EsRegalo { get; set; }
    }
}
