using Business.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASPAssignment.Controllers
{
    public class ProjectController(IProjectService projectService, IMemberService memberService) : Controller
    {

        private readonly IProjectService _projectService = projectService;
        private readonly IMemberService _memberService = memberService;


        [HttpGet]
        public async  Task<IActionResult> Create()
        {
            await LoadMembersToViewBag();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateForm form)
        {
            if (ModelState.IsValid)
            {
                await _projectService.CreateProjectAsync(form);
                return RedirectToAction("Index", "Home");
            }

            await LoadMembersToViewBag();
            return View(form);
        }
        private async Task LoadMembersToViewBag()
        {
            var members = await _memberService.GetAllMembers();

            ViewBag.Members = new MultiSelectList(
                members,
                "Id",
                "FullName"
            );
        }
    }
}
