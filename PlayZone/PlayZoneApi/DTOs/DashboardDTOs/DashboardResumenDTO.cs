namespace PlayZoneApi.DTOs.DashboardDTOs
{
    public class DashboardResumenDTO
    {
        // RESUMEN DE TRANSACCIONES
        public int TotalOrdenes { get; set; }
        public int OrdenesPendientes { get; set; }
        public int OrdenesCompletadas { get; set; }

        // INFORMACIÓN GENERAL
        public int TotalProductos { get; set; }
        public int TotalUsuarios { get; set; }

        // REEMBOLSOS
        public int TotalReembolsos { get; set; }
    }
}
