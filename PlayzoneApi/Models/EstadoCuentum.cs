using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class EstadoCuentum
{
    public int IdEstadoCuenta { get; set; }

    public string Estado { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
