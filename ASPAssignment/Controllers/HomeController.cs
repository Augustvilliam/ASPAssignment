using System.Diagnostics;
using ASPAssignment.Models;
using ASPAssignment.ViewModels;
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
        private const int PageSize = 10;

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
        public async Task<IActionResult> Index(string? status, int page = 1)
        {
            // 1) Profil-info till layout
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            ViewBag.ProfileImage = user.ProfileImagePath ?? "/img/Employee.svg";
            var profile = user.Profile;
            ViewBag.FullName = profile != null
                ? $"{profile.FirstName} {profile.LastName}".Trim()
                : "User";
            ViewBag.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User";
            ViewBag.Email = user.Email ?? "-";

            // 2) Members för picker i navbar (om du behöver den)
            var members = await _memberService.GetAllMembersAsync();
            ViewBag.Members = new MultiSelectList(members, "Id", "FullName");

            // 3) Paginering av projekt
            var total = await _projectService.CountAsync(status);
            var items = await _projectService.GetPagedAsync(
                status,
                (page - 1) * PageSize,
                PageSize
            );

            var vm = new ProjectIndex
            {
                Items = items,
                PageNumber = page,
                PageSize = PageSize,
                TotalItems = total,
                Status = status
            };

            return View(vm);
        }

        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
    }
}
