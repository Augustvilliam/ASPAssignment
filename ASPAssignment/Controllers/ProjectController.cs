using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASPAssignment.Controllers;

[Route("Project")]
public class ProjectController(IProjectService projectService, IMemberService memberService) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly IMemberService _memberService = memberService;

    [HttpPost("Create")]
    public async Task<IActionResult> Create(ProjectCreateForm form)
    {
        if (!ModelState.IsValid)
        {
            await LoadMembersToViewBag();

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                });

            return BadRequest(errors);
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

        return Json(new { success = true });
    }

    [HttpPost("Update")]
    public async Task<IActionResult> Update(ProjectEditForm form)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                });

            return BadRequest(errors);
        }

        string? imagePath = form.ExistingImagePath;

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
            Id = Guid.Parse(form.Id),
            ProjectName = form.ProjectName,
            ClientName = form.ClientName,
            Description = form.Description,
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            Budget = form.Budget,
            ProjectImagePath = imagePath,
            MemberIds = form.SelectedMemberId
        };

        var result = await _projectService.UpdateProjectAsync(dto);
        if (result)
            return Json(new { success = true });

        return StatusCode(500, new { message = "Failed to update project." });
    }

    [HttpGet("GetProject/{id}")]
    public async Task<IActionResult> GetProject(string id)
    {
        if (!Guid.TryParse(id, out Guid projectId))
            return BadRequest(new { message = "Invalid project ID format." });

        var project = await _projectService.GetProjectByIdAsync(projectId);
        if (project == null)
            return NotFound();

        return Json(project);
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
