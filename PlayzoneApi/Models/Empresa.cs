using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Empresa
{
    public int IdEmpresa { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdPais { get; set; }

    public string? SitioWeb { get; set; }

    public virtual Pai IdPaisNavigation { get; set; } = null!;

    public virtual ICollection<ProductosEmpresa> ProductosEmpresas { get; set; } = new List<ProductosEmpresa>();
}
