using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Reseña
{
    public int IdReseña { get; set; }

    public int IdUsuario { get; set; }

    public int IdProducto { get; set; }

    public bool EsRecomendado { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaPublicacion { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
