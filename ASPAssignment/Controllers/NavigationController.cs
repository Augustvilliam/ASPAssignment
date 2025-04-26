using ASPAssignment.ViewModels;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers
{
    [Authorize]
    [Route("Navigation")]
    public class NavigationController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IProjectService _projectService;
        private const int PageSize = 10;

        public NavigationController(IMemberService memberService, IProjectService projectService)
        {
            _memberService = memberService;
            _projectService = projectService;
        }

        [HttpGet("LoadProjects")]
        public async Task<IActionResult> LoadProjects(string? status, int page = 1)
        {
            var total = await _projectService.CountAsync(status);
            var items = await _projectService.GetPagedAsync(status, (page - 1) * PageSize, PageSize);

            var vm = new ProjectIndex
            {
                Items = items,
                PageNumber = page,
                PageSize = PageSize,
                TotalItems = total,
                Status = status
            };

            return PartialView("~/Views/Shared/Partials/Home/_ProjectView.cshtml", vm);
        }

        [HttpGet("LoadMembers")]
        public async Task<IActionResult> LoadMembers(int page = 1)
        {
            var total = await _memberService.CountAsync();
            var items = await _memberService.GetPagedAsync((page - 1) * PageSize, PageSize);

            var vm = new MemberIndex
            {
                Items = items,
                PageNumber = page,
                PageSize = PageSize,
                TotalItems = total
            };

            return PartialView("~/Views/Shared/Partials/Home/_MemberView.cshtml", vm);
        }
    }
}
