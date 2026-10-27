using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class ListaDeseado
{
    public int IdListaDeseados { get; set; }

    public int IdUsuario { get; set; }

    public int IdProducto { get; set; }

    public DateTime FechaAgregado { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
