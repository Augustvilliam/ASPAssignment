using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers
{
    public class AdminController : Controller
    {
        [Authorize]

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]

        public IActionResult Login()
        {
            return View();
        }
    }
}
