using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class EstadosOrden
{
    public int IdEstadoOrden { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<OrdenesCompra> OrdenesCompras { get; set; } = new List<OrdenesCompra>();
}
