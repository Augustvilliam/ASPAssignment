using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASPAssignment.Controllers
{
    public class ProjectController(IProjectService projectService, IMemberService memberService) : Controller
    {
        private readonly IProjectService _projectService = projectService;
        private readonly IMemberService _memberService = memberService;

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadMembersToViewBag();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateForm form)
        {
            if (ModelState.IsValid)
            {
                string? imagePath = null;

                if (form.ProjectImage != null && form.ProjectImage.Length > 0)
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
                    MemberIds = form.SelectedMembersIds
                };

                await _projectService.CreateProjectAsync(dto);
                return RedirectToAction("Index", "Home");
            }

            await LoadMembersToViewBag();
            return View(form);
        }

        private async Task LoadMembersToViewBag()
        {
            var members = await _memberService.GetAllMembersAsync();

            ViewBag.Members = new MultiSelectList(
                members,
                "Id",
                "FullName"
            );
        }
    }
}
