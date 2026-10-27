using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Etiqueta
{
    public int IdEtiqueta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<ProductosEtiqueta> ProductosEtiqueta { get; set; } = new List<ProductosEtiqueta>();
}
