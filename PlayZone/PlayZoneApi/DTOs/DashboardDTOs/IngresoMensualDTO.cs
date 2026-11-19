namespace PlayZoneApi.DTOs.DashboardDTOs
{
    public class IngresoMensualDTO
    {
        public int Anio { get; set; }

        public int Mes { get; set; }

        public string NombreMes { get; set; }

        public int CantidadOrdenes { get; set; }

        public int JuegosVendidos { get; set; }

        public decimal MontoBruto { get; set; }

        public decimal Descuentos { get; set; }

        public decimal IngresoNeto { get; set; }
        public string Moneda { get; set; }

        public string CodigoMoneda { get; set; }
    }
}
