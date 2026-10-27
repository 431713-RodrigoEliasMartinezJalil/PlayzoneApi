using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class OrdenesCompra
{
    public int IdOrden { get; set; }

    public int IdUsuario { get; set; }

    public int IdMetodoPago { get; set; }

    public int IdMoneda { get; set; }

    public int IdEstadoOrden { get; set; }

    public DateTime FechaOrden { get; set; }

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();

    public virtual EstadosOrden IdEstadoOrdenNavigation { get; set; } = null!;

    public virtual MetodosPago IdMetodoPagoNavigation { get; set; } = null!;

    public virtual Moneda IdMonedaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
