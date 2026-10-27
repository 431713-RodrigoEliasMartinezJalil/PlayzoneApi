using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class PreciosProducto
{
    public int IdPrecioProducto { get; set; }

    public int IdProducto { get; set; }

    public int IdMoneda { get; set; }

    public decimal PrecioMonto { get; set; }

    public DateTime FechaInicioVigencia { get; set; }

    public DateTime? FechaFinVigencia { get; set; }

    public virtual Moneda IdMonedaNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
