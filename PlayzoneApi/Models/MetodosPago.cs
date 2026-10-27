using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class MetodosPago
{
    public int IdMetodoPago { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<OrdenesCompra> OrdenesCompras { get; set; } = new List<OrdenesCompra>();
}
