using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public int? IdProductoPadre { get; set; }

    public int IdTipoProducto { get; set; }

    public int IdEstadoPublicacion { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string ClasificacionEdad { get; set; } = null!;

    public DateOnly FechaLanzamiento { get; set; }

    public virtual ICollection<BibliotecaUsuario> BibliotecaUsuarios { get; set; } = new List<BibliotecaUsuario>();

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();

    public virtual EstadosPublicacion IdEstadoPublicacionNavigation { get; set; } = null!;

    public virtual Producto? IdProductoPadreNavigation { get; set; }

    public virtual TiposProducto IdTipoProductoNavigation { get; set; } = null!;

    public virtual ICollection<Producto> InverseIdProductoPadreNavigation { get; set; } = new List<Producto>();

    public virtual ICollection<ListaDeseado> ListaDeseados { get; set; } = new List<ListaDeseado>();

    public virtual ICollection<PreciosProducto> PreciosProductos { get; set; } = new List<PreciosProducto>();

    public virtual ICollection<ProductosEmpresa> ProductosEmpresas { get; set; } = new List<ProductosEmpresa>();

    public virtual ICollection<ProductosEtiqueta> ProductosEtiqueta { get; set; } = new List<ProductosEtiqueta>();

    public virtual ICollection<ProductosGenero> ProductosGeneros { get; set; } = new List<ProductosGenero>();

    public virtual ICollection<PromocionesProducto> PromocionesProductos { get; set; } = new List<PromocionesProducto>();

    public virtual ICollection<Reseña> Reseñas { get; set; } = new List<Reseña>();
}
