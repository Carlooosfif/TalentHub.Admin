using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentHub.Admin.Models
{
    // OJO: esta clase ahora representa la evaluación general
    // de un empleado por su supervisor (no por vacante específica).
    public class EvaluacionVacante
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Empleado")]
        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }

        [Required]
        [ForeignKey("Supervisor")]
        public int SupervisorId { get; set; }
        public Supervisor? Supervisor { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "El score debe estar entre 0 y 100.")]
        [Display(Name = "Score del supervisor")]
        public int ScoreSupervisor { get; set; }

        [MaxLength(500)]
        [Display(Name = "Comentarios del supervisor")]
        public string? Comentarios { get; set; }

        [Display(Name = "Fecha de evaluación")]
        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;
    }
}
