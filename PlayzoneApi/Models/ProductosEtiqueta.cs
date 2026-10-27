using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class ProductosEtiqueta
{
    public int IdProductoEtiqueta { get; set; }

    public int IdProducto { get; set; }

    public int IdEtiqueta { get; set; }

    public virtual Etiqueta IdEtiquetaNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
