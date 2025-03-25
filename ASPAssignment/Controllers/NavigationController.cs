using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class NavigationController : Controller
{
    public IActionResult LoadProjects()
    {
        return PartialView("/Partials/_ProjectView");
    }
    public IActionResult LoadTeamMembers()
    {
        return PartialView("/Partials/_TeamMembers");
    }
}
