using TalentHub.Admin.Models;
using TalentHub.Admin.Strategies;
using Xunit;

namespace TalentHub.Admin.Tests
{
    public class StrategiesTests
    {
        [Fact]
        public void EvaluacionStrategy_CalcularScore_CuandoScoreSupervisorEsNull_RetornaCero()
        {
            // Arrange
            var strategy = new EvaluacionStrategy();
            var model = new RecomendacionEmpleadoViewModel
            {
                ScoreSupervisor = null
            };

            // Act
            var score = strategy.CalcularScore(model);

            // Assert
            Assert.Equal(0, score);
        }

        [Fact]
        public void EvaluacionStrategy_CalcularScore_CuandoScoreSupervisorTieneValor_RetornaMismoValor()
        {
            // Arrange
            var strategy = new EvaluacionStrategy();
            var model = new RecomendacionEmpleadoViewModel
            {
                ScoreSupervisor = 5
            };

            // Act
            var score = strategy.CalcularScore(model);

            // Assert
            Assert.Equal(5, score);
        }
    }
}
