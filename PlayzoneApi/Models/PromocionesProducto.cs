using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class PromocionesProducto
{
    public int IdPromocionProducto { get; set; }

    public int IdPromocion { get; set; }

    public int IdProducto { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Promocione IdPromocionNavigation { get; set; } = null!;
}
