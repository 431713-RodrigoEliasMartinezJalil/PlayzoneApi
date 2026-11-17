namespace PlayZoneApi.DTOs
{
    public class OrdenCompraDTO
    {
        public int IdOrden { get; set; }

        public int IdUsuario { get; set; }

        public string NombreUsuario { get; set; }

        public string EmailUsuario { get; set; }

        public DateTime FechaOrden { get; set; }

        public string MetodoPago { get; set; }

        public string Moneda { get; set; }

        public string CodigoMoneda { get; set; }

        public string EstadoOrden { get; set; }

        public List<DetalleOrdenDTO> Detalles { get; set; }
    }
}
