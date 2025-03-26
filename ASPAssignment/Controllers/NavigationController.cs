using Business.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class NavigationController : Controller
{
    private readonly IMemberService _memberService;

    public NavigationController(IMemberService memberService)
    {
        _memberService = memberService;
    }
    public IActionResult LoadProjects()
    {
        return PartialView("~/Views/Shared/Partials/_ProjectView.cshtml");
    }
    public async Task<IActionResult> LoadTeamMembers()
    {
        var members = await _memberService.GetAllMembers();
        return PartialView("~/Views/Shared/Partials/_TeamMembers.cshtml", members);
    }
}
