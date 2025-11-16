using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentHub.Admin.Models
{
    public class Supervisor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [ForeignKey("Area")]
        [Display(Name = "Área")]
        public int AreaId { get; set; }
        public Area? Area { get; set; }

        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
