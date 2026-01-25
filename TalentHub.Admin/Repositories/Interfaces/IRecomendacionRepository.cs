using TalentHub.Admin.Models;

namespace TalentHub.Admin.Repositories.Interfaces
{
    public interface IRecomendacionRepository
    {
        List<RecomendacionEmpleadoViewModel> ObtenerCandidatosPorArea(string area);
    }
}