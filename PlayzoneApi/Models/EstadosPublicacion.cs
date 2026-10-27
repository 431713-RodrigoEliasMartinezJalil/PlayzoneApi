using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class EstadosPublicacion
{
    public int IdEstadoPublicacion { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
