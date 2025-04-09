using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
