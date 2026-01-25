using TalentHub.Admin.Models;

namespace TalentHub.Admin.Services.Interfaces
{
    public interface IRecomendacionService
    {
        List<RecomendacionEmpleadoViewModel> ObtenerRanking(int vacanteId);
    }
}