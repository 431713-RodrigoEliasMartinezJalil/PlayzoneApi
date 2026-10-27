using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Promocione
{
    public int IdPromocion { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal PorcentajeDescuento { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public bool Activa { get; set; }

    public virtual ICollection<PromocionesProducto> PromocionesProductos { get; set; } = new List<PromocionesProducto>();
}
