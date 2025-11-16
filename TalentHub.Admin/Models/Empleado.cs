using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentHub.Admin.Models
{
    public class Empleado
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos.")]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        public DateTime FechaIngreso { get; set; }

        [ForeignKey("Area")]
        [Display(Name = "Área")]
        public int AreaId { get; set; }
        public Area? Area { get; set; }

        [ForeignKey("Supervisor")]
        [Display(Name = "Supervisor")]
        public int SupervisorId { get; set; }
        public Supervisor? Supervisor { get; set; }
    }
}
