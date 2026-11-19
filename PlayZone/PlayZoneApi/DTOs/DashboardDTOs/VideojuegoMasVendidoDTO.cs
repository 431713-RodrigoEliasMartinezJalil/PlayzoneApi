namespace PlayZoneApi.DTOs.DashboardDTOs
{
    public class VideojuegoMasVendidoDTO
    {
        public int IdProducto { get; set; }

        public string Titulo { get; set; }

        public int CantidadVentas { get; set; }

        public decimal IngresosGenerados { get; set; }
    }
}
