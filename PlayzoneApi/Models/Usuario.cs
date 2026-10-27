using System;
using System.Collections.Generic;

namespace PlayzoneApi.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public DateOnly FechaNacimiento { get; set; }

    public int IdPais { get; set; }

    public string? Biografia { get; set; }

    public string? AvatarUrl { get; set; }

    public int IdEstadoCuenta { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<BibliotecaUsuario> BibliotecaUsuarios { get; set; } = new List<BibliotecaUsuario>();

    public virtual EstadoCuentum IdEstadoCuentaNavigation { get; set; } = null!;

    public virtual Pai IdPaisNavigation { get; set; } = null!;

    public virtual ICollection<ListaDeseado> ListaDeseados { get; set; } = new List<ListaDeseado>();

    public virtual NivelesUsuario? NivelesUsuario { get; set; }

    public virtual ICollection<OrdenesCompra> OrdenesCompras { get; set; } = new List<OrdenesCompra>();

    public virtual ICollection<Reseña> Reseñas { get; set; } = new List<Reseña>();
}
