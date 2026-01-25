using System;

namespace TalentHub.Admin.Models
{
    public class RecomendacionEmpleadoViewModel
    {
        public int EmpleadoId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        // Score del supervisor para esta vacante (puede ser null si aún no se evalúa)
        public int? ScoreSupervisor { get; set; }
        public int PuntajeFinal { get; set; }
    }
}
