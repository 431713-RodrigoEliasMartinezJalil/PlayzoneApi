using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class EstadosReembolso
{
    public int IdEstadoReembolso { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Reembolso> Reembolsos { get; set; } = new List<Reembolso>();
}
