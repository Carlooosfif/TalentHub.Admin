using Microsoft.AspNetCore.Mvc;

namespace TalentHub.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
