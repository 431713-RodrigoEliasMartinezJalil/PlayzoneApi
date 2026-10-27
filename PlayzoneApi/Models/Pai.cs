using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Pai
{
    public int IdPais { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Empresa> Empresas { get; set; } = new List<Empresa>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
