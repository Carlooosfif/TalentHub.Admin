using Microsoft.AspNetCore.Mvc;
using TalentHub.Admin.Controllers;
using TalentHub.Admin.Services.Interfaces;

public class RecomendacionesController : BaseController
{
    private readonly IRecomendacionService _service;

    public RecomendacionesController(IRecomendacionService service)
    {
        _service = service;
    }

    public IActionResult Index(int vacanteId)
    {
        var r = Proteger();
        if (r != null) return r;
        var ranking = _service.ObtenerRanking(vacanteId);
        ViewBag.VacanteId = vacanteId;
        return View(ranking);
    }
}
