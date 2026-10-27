using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class DetalleOrden
{
    public int IdDetalleOrden { get; set; }

    public int IdOrden { get; set; }

    public int IdProducto { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal MontoDescuento { get; set; }

    public bool EsRegalo { get; set; }

    public virtual OrdenesCompra IdOrdenNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Reembolso? Reembolso { get; set; }
}
