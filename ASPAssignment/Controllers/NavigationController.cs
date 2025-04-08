using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class NavigationController : Controller
{
    private readonly IMemberService _memberService;
    private readonly IProjectService _projectService;

    public NavigationController(IMemberService memberService, IProjectService projectService)
    {
        _memberService = memberService;
        _projectService = projectService;
    }
    [Authorize]
    public async Task<IActionResult> LoadProjects(string? status)
    {
        var allProjects = await _projectService.GetAllProjectsAsync();

        if (!string.IsNullOrEmpty(status) && (status == "Ongoing" || status == "Completed"))
            allProjects = allProjects.Where(p => p.Status == status);

        if (!allProjects.Any())
            return Content("No projects found.");

        return PartialView("~/Views/Shared/Partials/_ProjectView.cshtml", allProjects);
    }

    [Authorize]

    public async Task<IActionResult> LoadTeamMembers()
    {
        var members = await _memberService.GetAllMembersAsync();
        return PartialView("~/Views/Shared/Partials/_TeamMembers.cshtml", members);
    }
}
