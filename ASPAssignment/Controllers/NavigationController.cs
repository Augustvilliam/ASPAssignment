using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class NavigationController : Controller
{
    public IActionResult LoadProjects()
    {
        return PartialView("_ProjectView");
    }
    public IActionResult LoadTeamMembers()
    {
        return PartialView("_TeamMembers");
    }
}
