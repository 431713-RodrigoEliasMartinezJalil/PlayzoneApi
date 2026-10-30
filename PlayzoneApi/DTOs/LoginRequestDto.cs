namespace PlayzoneApi.DTOs
{
    public class LoginRequestDto
    {
        public string EmailOrUsername { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class UsuarioSesionDto
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? NombreCompleto { get; set; }
        public string Pais { get; set; } = null!;
        public string EstadoCuenta { get; set; } = null!;
        public int Nivel { get; set; }
        public int Experiencia { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
