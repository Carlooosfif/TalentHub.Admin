using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TalentHub.Admin.Models
{
    public class Area
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Supervisor> Supervisores { get; set; } = new List<Supervisor>();
        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
