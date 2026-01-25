using TalentHub.Admin.Models;
using TalentHub.Admin.Strategies.Interfaces;

namespace TalentHub.Admin.Strategies
{
    public class EvaluacionStrategy : ICandidatoScoringStrategy
    {
        public int CalcularScore(RecomendacionEmpleadoViewModel candidato)
        {
            return candidato.ScoreSupervisor ?? 0;
        }
    }
}

