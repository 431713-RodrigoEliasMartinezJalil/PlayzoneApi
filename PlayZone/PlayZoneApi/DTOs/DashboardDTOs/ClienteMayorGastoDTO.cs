namespace PlayZoneApi.DTOs.DashboardDTOs
{
    public class ClienteMayorGastoDTO
    {
        public int IdUsuario { get; set; }

        public string NombreUsuario { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public int IdMoneda { get; set; }

        public int CantidadCompras { get; set; }

        public decimal TotalGastado { get; set; }

        public decimal TicketPromedio { get; set; }
    }
}