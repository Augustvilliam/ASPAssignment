using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class ExternalController : Controller
{
    public IActionResult ExternalRegister()
    {
        return View();
    }

    public IActionResult ExternalSignIn()
    {
        return View();
    }
}
