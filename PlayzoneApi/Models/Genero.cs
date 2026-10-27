using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Genero
{
    public int IdGenero { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<ProductosGenero> ProductosGeneros { get; set; } = new List<ProductosGenero>();
}
