using ASPAssignment.Services;
using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASPAssignment.Controllers;

[Route("Project")]
[Authorize] // minst var inloggad 
public class ProjectController(IProjectService projectService,
    IMemberService memberService,
    INotificationService notificationService) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly IMemberService _memberService = memberService;
    private readonly INotificationService _notificationService = notificationService;

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
            MemberIds = form.SelectedMemberId,
            Status = form.Status
        };

        await _projectService.CreateProjectAsync(dto);

        // Hämta admin som tilldelar
        var assigningUser = await _memberService.GetMemberByEmailAsync(User.Identity.Name);
        var assignerName = $"{assigningUser.FirstName} {assigningUser.LastName}".Trim();

        // Hämta medlemmar som tilldelats
        var assignedMembers = await _memberService.GetAllMembersAsync();
        var selectedMembers = assignedMembers
            .Where(m => form.SelectedMemberId.Contains(m.Id))
            .ToList();

        // Skicka en notis till varje tilldelad medlem
        foreach (var member in selectedMembers)
        {
            var notification = new NotificationDto
            {
                ImageUrl = assigningUser.ProfileImagePath ?? "/img/default-user.svg",
                Message = $"{assignerName} har tilldelat dig projektet {form.ProjectName}",
                Timestamp = DateTime.UtcNow,
                NotificationId = Guid.NewGuid().ToString(),
                NotificationType = "ProjectAssigned"
            };

            await _notificationService.SendNotificationAsync(member.Email, notification);
        }

        return Json(new { success = true });
    }

    [HttpPost("Update")]
    [Authorize(Policy = "RequireProjectLeadOrAppAdmin")] // Endast projektledare eller appadmin kan uppdatera
    public async Task<IActionResult> Update(ProjectEditForm form)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new {
                    Field = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                });
            return BadRequest(errors);
        }

        //Hämta befintligt projekt DTO för att få gamla medlemmar
        var projectId = Guid.Parse(form.Id);
        var existing = await _projectService.GetProjectByIdAsync(projectId);
        var oldMemberIds = existing?.MemberIds ?? Enumerable.Empty<string>();

        //Hantera eventuell ny bild
        string? imagePath = form.ExistingImagePath;
        if (form.ProjectImage is { Length: > 0 })
        {
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadDir);
            var fileName = $"{Guid.NewGuid()}_{form.ProjectImage.FileName}";
            var fullPath = Path.Combine(uploadDir, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await form.ProjectImage.CopyToAsync(stream);
            imagePath = $"/uploads/{fileName}";
        }

        //Bygg DTO och uppdatera
        var dto = new ProjectDto
        {
            Id = projectId,
            ProjectName = form.ProjectName,
            ClientName = form.ClientName,
            Description = form.Description,
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            Budget = form.Budget,
            ProjectImagePath = imagePath,
            MemberIds = form.SelectedMemberId,
            Status = form.Status
        };

        var ok = await _projectService.UpdateProjectAsync(dto);
        if (!ok)
            return StatusCode(500, new { message = "Failed to update project." });

        //Skicka notis till nytillagda medlemmar
        var addedMemberIds = dto.MemberIds.Except(oldMemberIds);
        var changer = await _memberService.GetMemberByEmailAsync(User.Identity.Name!);
        var changerName = $"{changer.FirstName} {changer.LastName}".Trim();

        foreach (var memberId in addedMemberIds)
        {
            var member = await _memberService.GetMemberByIdAsync(memberId);
            if (member == null) continue;
            var note = new NotificationDto
            {
                ImageUrl = changer.ProfileImagePath ?? "/img/default-user.svg",
                Message = $"{changerName} har lagt till dig i projektet \"{dto.ProjectName}\"",
                Timestamp = DateTime.UtcNow,
                NotificationType = "ProjectMemberAdded"
            };
            await _notificationService.SendNotificationAsync(member.Email, note);
        }

        //Skicka status-notification 
        var statusNote = new NotificationDto
        {
            ImageUrl = changer.ProfileImagePath ?? "/img/default-user.svg",
            Message = $"{changerName} har ändrat status på \"{dto.ProjectName}\" till {dto.Status}",
            Timestamp = DateTime.UtcNow,
            NotificationType = "StatusChanged"
        };
        var admins = await _memberService.GetAllAdminsAsync();
        foreach (var admin in admins)
            await _notificationService.SendNotificationAsync(admin.Email, statusNote);

        return Json(new { success = true });
    }


    [HttpGet("GetProject/{id}")]
    public async Task<IActionResult> GetProject(string id)
    {
        if (!Guid.TryParse(id, out Guid projectId))
            return BadRequest(new { message = "Invalid project ID format." });

        var project = await _projectService.GetProjectByIdAsync(projectId);
        if (project == null)
            return NotFound();

        // Hämta medlemmar separat från service
        var members = await _memberService.GetAllMembersAsync();
        var assignedMembers = members
            .Where(m => project.MemberIds.Contains(m.Id))
            .Select(m => new
            {
                id = m.Id,
                fullName = $"{m.FirstName} {m.LastName}",
                avatarUrl = Url.Content(m.ProfileImagePath ?? "/images/default-avatar.png")
            })
            .ToList();

        return Json(new
        {
            project.Id,
            project.ProjectName,
            project.ClientName,
            project.Description,
            project.StartDate,
            project.EndDate,
            project.Budget,
            project.Status,
            project.ProjectImagePath,
            members = assignedMembers
        });
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Policy = "RequireAppAdmin")] // Endast appadmin kan ta bort projekt
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _projectService.DeleteProjectAsync( id);
        if (!result)
            return NotFound(new { message = "Project Not Found" });
        return Ok(new { sucess = true });
    }

    [HttpPost]
    [Authorize(Policy = "RequireProjectLeadOrAppAdmin")]
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
