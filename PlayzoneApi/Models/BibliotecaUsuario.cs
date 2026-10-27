using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class BibliotecaUsuario
{
    public int IdBiblioteca { get; set; }

    public int IdUsuario { get; set; }

    public int IdProducto { get; set; }

    public DateTime FechaAdquisicion { get; set; }

    public string OrigenAdquisicion { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
