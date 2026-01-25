using TalentHub.Admin.Models;

namespace TalentHub.Admin.Strategies.Interfaces
{
    public interface ICandidatoScoringStrategy
    {
        int CalcularScore(RecomendacionEmpleadoViewModel candidato);
    }
}