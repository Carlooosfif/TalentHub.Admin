using TalentHub.Admin.Models;
using TalentHub.Admin.Repositories.Interfaces;
using TalentHub.Admin.Services.Interfaces;
using TalentHub.Admin.Strategies.Interfaces;

namespace TalentHub.Admin.Services
{
    public class RecomendacionService : IRecomendacionService
    {
        private readonly IVacanteRepository _vacanteRepo;
        private readonly IRecomendacionRepository _recomendacionRepo;
        private readonly IEnumerable<ICandidatoScoringStrategy> _strategies;

        public RecomendacionService(
            IVacanteRepository vacanteRepo,
            IRecomendacionRepository recomendacionRepo,
            IEnumerable<ICandidatoScoringStrategy> strategies)
        {
            _vacanteRepo = vacanteRepo;
            _recomendacionRepo = recomendacionRepo;
            _strategies = strategies;
        }

        public List<RecomendacionEmpleadoViewModel> ObtenerRanking(int vacanteId)
        {
            var vacante = _vacanteRepo.ObtenerPorId(vacanteId);
            if (vacante == null)
                return new List<RecomendacionEmpleadoViewModel>();

            var candidatos = _recomendacionRepo.ObtenerCandidatosPorArea(vacante.Area);

            foreach (var c in candidatos)
            {
                int total = c.ScoreSupervisor ?? 0;

                foreach (var strategy in _strategies)
                {
                    total += strategy.CalcularScore(c);
                }

                c.ScoreSupervisor = total;
            }

            return candidatos
                .OrderByDescending(c => c.ScoreSupervisor)
                .ToList();
        }
    }
}
