using TalentHub.Admin.Models;

namespace TalentHub.Admin.Repositories.Interfaces
{
    public interface IVacanteRepository
    {
        Vacante? ObtenerPorId(int id);
    }
}