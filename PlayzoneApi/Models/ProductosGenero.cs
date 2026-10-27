using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class ProductosGenero
{
    public int IdProductoGenero { get; set; }

    public int IdProducto { get; set; }

    public int IdGenero { get; set; }

    public virtual Genero IdGeneroNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
