using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class NivelesUsuario
{
    public int IdNivelUsuario { get; set; }

    public int IdUsuario { get; set; }

    public int Experiencia { get; set; }

    public int Nivel { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
