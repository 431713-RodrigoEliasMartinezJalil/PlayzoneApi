namespace PlayzoneApi.DTOs
{
    public class SolicitudReembolsoRequestDto
    {
        public int IdUsuario { get; set; }
        public int IdDetalleOrden { get; set; }
        public string Motivo { get; set; } = null!;
    }

    public class CrearReseñaRequestDto
    {
        public int IdUsuario { get; set; }
        public int IdProducto { get; set; }
        public bool EsRecomendado { get; set; }
        public string? Comentario { get; set; }
    }

    public class ResumenVentaDto
    {
        public int NroOrden { get; set; }
        public DateTime FechaCompra { get; set; }
        public string Usuario { get; set; } = null!;
        public string Videojuego { get; set; } = null!;
        public string MetodoPago { get; set; } = null!;
        public decimal ImporteCobrado { get; set; }
    }
    public class IngresoMensualDto
    {
        public int Mes { get; set; }
        public int CantidadOrdenes { get; set; }
        public int JuegosVendidos { get; set; }
        public decimal MontoBruto { get; set; }
        public decimal Descuentos { get; set; }
        public decimal IngresoNeto { get; set; }
    }
}
