using Microsoft.EntityFrameworkCore;
using PlayzoneApi.DTOs;
using PlayzoneApi.Models;
using PlayzoneApi.Repositories.Interfaces;

namespace PlayzoneApi.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly PlayZoneContext _context;
        public DashboardRepository(PlayZoneContext context)
        {
            _context = context;
        }

        public async Task<List<ResumenVentaDto>> ObtenerResumenVentasAnioActualAsync()
        {
            return await _context.Database
                .SqlQuery<ResumenVentaDto>($@"
                SELECT 
                    Nro_Orden AS NroOrden,
                    Fecha_Compra AS FechaCompra,
                    Usuario,
                    Videojuego,
                    Metodo_Pago AS MetodoPago,
                    Importe_Cobrado AS ImporteCobrado
                FROM vista_resumen_compras
                WHERE YEAR(Fecha_Compra) = YEAR(GETDATE())
                ORDER BY Fecha_Compra DESC")
                .ToListAsync();
        }

        public async Task<List<IngresoMensualDto>> ObtenerIngresosMensualesAsync(int anio)
        {
            return await _context.Database
                .SqlQuery<IngresoMensualDto>($@"
                SELECT 
                    mes AS Mes,
                    cantidad_ordenes AS CantidadOrdenes,
                    juegos_vendidos AS JuegosVendidos,
                    monto_bruto AS MontoBruto,
                    descuentos AS Descuentos,
                    ingreso_neto AS IngresoNeto
                FROM dbo.fn_Ingresos_Mensuales({anio})
                ORDER BY mes")
                .ToListAsync();
        }
    }
}
