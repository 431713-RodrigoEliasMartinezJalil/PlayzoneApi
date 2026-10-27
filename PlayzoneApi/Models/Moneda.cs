using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Moneda
{
    public int IdMoneda { get; set; }

    public string Nombre { get; set; } = null!;

    public string Codigo { get; set; } = null!;

    public virtual ICollection<OrdenesCompra> OrdenesCompras { get; set; } = new List<OrdenesCompra>();

    public virtual ICollection<PreciosProducto> PreciosProductos { get; set; } = new List<PreciosProducto>();
}
