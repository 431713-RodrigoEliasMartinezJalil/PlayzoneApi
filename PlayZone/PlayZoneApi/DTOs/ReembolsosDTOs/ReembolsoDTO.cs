namespace PlayZoneApi.DTOs.ReembolsoDTOs
{
    public class ReembolsoDTO
    {
        public int IdReembolso { get; set; }

        public int IdDetalleOrden { get; set; }

        public string NombreProducto { get; set; }

        public string EstadoReembolso { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public string Motivo { get; set; }

        public decimal MontoReembolsado { get; set; }
    }
}
