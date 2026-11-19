namespace PlayZoneApi.DTOs.DashboardDTOs
{
    public class DashboardResumenDTO
    {
        public int ProductosPublicados { get; set; }

        public int TotalOrdenes { get; set; }

        public int OrdenesPendientes { get; set; }

        public int OrdenesCompletadas { get; set; }

        public int ReembolsosPendientes { get; set; }

        public int TotalUsuarios { get; set; }

        // REEMBOLSOS
        public int TotalReembolsos { get; set; }

        public List<VentasPorMonedaDTO> VentasPorMoneda { get; set; }
    }

    public class VentasPorMonedaDTO
    {
        public string Moneda { get; set; }

        public string CodigoMoneda { get; set; }

        public decimal TotalVendido { get; set; }
    }
}