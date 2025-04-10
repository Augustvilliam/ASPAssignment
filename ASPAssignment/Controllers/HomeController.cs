using System.Diagnostics;
using ASPAssignment.Models;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASPAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProjectService _projectService;
        private readonly IMemberService _memberService;
        private readonly UserManager<MemberEntity> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            IProjectService projectService,
            IMemberService memberService,
            UserManager<MemberEntity> userManager)
        {
            _logger = logger;
            _projectService = projectService;
            _memberService = memberService;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var profile = user.Profile;

            ViewBag.ProfileImage = user.ProfileImagePath ?? "/img/Employee.svg";
            ViewBag.FullName = profile != null ? $"{profile.FirstName} {profile.LastName}".Trim() : "User";

            ViewBag.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User";
            ViewBag.Email = user.Email ?? "-";

            var members = await _memberService.GetAllMembersAsync();
            ViewBag.Members = new MultiSelectList(members, "Id", "FullName");

            var projects = await _projectService.GetAllProjectsAsync();
            return View(projects);
        }

        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}