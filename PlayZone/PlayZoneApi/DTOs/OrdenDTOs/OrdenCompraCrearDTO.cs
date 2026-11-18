namespace PlayZoneApi.DTOs.OrdenDTOs
{
    public class OrdenCompraCrearDTO
    {
        public int IdUsuario { get; set; }
        public int IdMetodoPago { get; set; }
        public int IdMoneda { get; set; }

        public List<DetalleOrdenCrearDTO> Detalles { get; set; }
    }

    public class DetalleOrdenCrearDTO
    {
        public int IdProducto { get; set; }
        public decimal MontoDescuento { get; set; }
        public bool EsRegalo { get; set; }
    }
}