using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class ProductosEmpresa
{
    public int IdProductoEmpresa { get; set; }

    public int IdProducto { get; set; }

    public int IdEmpresa { get; set; }

    public string Rol { get; set; } = null!;

    public virtual Empresa IdEmpresaNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
