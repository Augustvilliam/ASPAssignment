using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASPAssignment.Controllers;

public class ProjectController(IProjectService projectService, IMemberService memberService) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly IMemberService _memberService = memberService;

    [HttpPost]
    public async Task<IActionResult> Create(ProjectCreateForm form)
    {
        await LoadMembersToViewBag();

        if (!ModelState.IsValid)
        {
            // Återrenderar samma partial (modal) med valideringsfel
            return PartialView("Partials/Modals/_CreateProjectModal", form);
        }

        string? imagePath = null;
        if (form.ProjectImage is { Length: > 0 })
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}_{form.ProjectImage.FileName}";
            var filePath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await form.ProjectImage.CopyToAsync(stream);

            imagePath = $"/uploads/{fileName}";
        }

        var dto = new ProjectDto
        {
            ProjectName = form.ProjectName,
            ClientName = form.ClientName,
            Description = form.Description,
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            Budget = form.Budget,
            ProjectImagePath = imagePath,
            MemberIds = form.SelectedMemberId
        };

        await _projectService.CreateProjectAsync(dto);
        return RedirectToAction("Index", "Home");
    }

    private async Task LoadMembersToViewBag()
    {
        var members = await _memberService.GetAllMembersAsync();

        ViewBag.Member = new SelectList(
            members,
            "Id",
            "FullName"
        );
    }
}
