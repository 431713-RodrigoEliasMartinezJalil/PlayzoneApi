using System.ComponentModel.DataAnnotations;

namespace PlayZoneApi.DTOs.UsuarioDTOs
{
    public class UserRegisterDTO
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public DateOnly BirthDate { get; set; }

        [Required]
        public int CountryId { get; set; }
    }
}
