using System.ComponentModel.DataAnnotations;

namespace TalentHub.Admin.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Supervisor"; // Admin o Supervisor

        [Display(Name = "Supervisor asociado (solo si rol = Supervisor)")]
        public int? SupervisorId { get; set; }
    }
}
