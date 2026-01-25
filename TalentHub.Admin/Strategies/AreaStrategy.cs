using TalentHub.Admin.Models;
using TalentHub.Admin.Strategies.Interfaces;

namespace TalentHub.Admin.Strategies
{
    public class AreaStrategy : ICandidatoScoringStrategy
    {
        public int CalcularScore(RecomendacionEmpleadoViewModel model)
        {
            return model.ScoreSupervisor ?? 0;
        }
    }
}
