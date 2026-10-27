using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class VistaResumenCompra
{
    public int NroOrden { get; set; }

    public DateTime FechaCompra { get; set; }

    public string Usuario { get; set; } = null!;

    public string Videojuego { get; set; } = null!;

    public string MetodoPago { get; set; } = null!;

    public decimal? ImporteCobrado { get; set; }
}
