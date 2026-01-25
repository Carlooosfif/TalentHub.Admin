using Microsoft.AspNetCore.Mvc;

namespace TalentHub.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            var r = Proteger();
            if (r != null) return r;
            return View();
        }
    }
}
