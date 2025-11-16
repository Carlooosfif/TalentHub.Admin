using System.ComponentModel.DataAnnotations;

namespace TalentHub.Admin.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Display(Name = "Contraseña")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Rol { get; set; } = "Admin";
    }
}
