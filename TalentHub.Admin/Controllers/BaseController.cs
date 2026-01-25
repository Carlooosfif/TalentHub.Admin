using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TalentHub.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected bool SesionActiva()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("Rol")
            );
        }

        protected IActionResult Proteger()
        {
            if (!SesionActiva())
                return RedirectToAction("Login", "Account");

            return null!;
        }
    }
}
