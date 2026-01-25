using TalentHub.Admin.Models;
using TalentHub.Admin.Strategies.Interfaces;

namespace TalentHub.Admin.Strategies
{
    public class AntiguedadStrategy : ICandidatoScoringStrategy
    {
        public int CalcularScore(RecomendacionEmpleadoViewModel candidato)
        {

            return 5;
        }
    }
}
