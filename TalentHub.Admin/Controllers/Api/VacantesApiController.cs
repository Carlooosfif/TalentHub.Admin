using Microsoft.AspNetCore.Mvc;
using TalentHub.Admin.Services.Interfaces;

namespace TalentHub.Admin.Controllers.Api
{
    [ApiController]
    [Route("api/vacantes")]
    public class VacantesApiController : ControllerBase
    {
        private readonly IRecomendacionService _recomendacionService;

        public VacantesApiController(IRecomendacionService recomendacionService)
        {
            _recomendacionService = recomendacionService;
        }

       
        [HttpGet("{id}/recomendaciones")]
        public IActionResult GetRecomendaciones(int id)
        {
            var resultado = _recomendacionService.ObtenerRanking(id);

            if (resultado == null || resultado.Count == 0)
                return NotFound(new
                {
                    mensaje = "No se encontraron recomendaciones para la vacante."
                });

            return Ok(resultado);
        }
    }
}
