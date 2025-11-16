using System;

namespace TalentHub.Admin.Models
{
    public class Vacante
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Area { get; set; }
        public string Ubicacion { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
