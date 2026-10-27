using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Reembolso
{
    public int IdReembolso { get; set; }

    public int IdDetalleOrden { get; set; }

    public int IdEstadoReembolso { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public string Motivo { get; set; } = null!;

    public decimal MontoReembolsado { get; set; }

    public virtual DetalleOrden IdDetalleOrdenNavigation { get; set; } = null!;

    public virtual EstadosReembolso IdEstadoReembolsoNavigation { get; set; } = null!;
}
