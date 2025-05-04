using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers
{//navigationcontroller specifikt för dynamiccontent delen.
    [Authorize]
    [Route("Navigation")]
    public class NavigationController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IProjectService _projectService;
        private readonly ITagService _tagService;
        private const int PageSize = 9;

        public NavigationController(IMemberService memberService,
            IProjectService projectService,
            ITagService tagService)
        {
            _memberService = memberService;
            _projectService = projectService;
            _tagService = tagService;
        }


        [HttpGet("LoadProjects")]
        public async Task<IActionResult> LoadProjects(string? status, string? term, int page = 1)
        {
            IEnumerable<ProjectDto> dtos;
            int total;

            if (!string.IsNullOrWhiteSpace(term))
            {
                dtos = await _tagService.SearchProjectsAsync(term);
                total = dtos.Count();
                page = 1;
            }
            else
            {
                total = await _projectService.CountAsync(status);
                dtos = await _projectService.GetPagedAsync(status, (page - 1) * PageSize, PageSize);
            }

            // ─── Hämta fasta totalsiffror ───
            var allCount = await _projectService.CountAsync(null);
            var ongoingCount = await _projectService.CountAsync("Ongoing");
            var completedCount = await _projectService.CountAsync("Completed");

            var vm = new ProjectIndex
            {
                Items = dtos,
                PageNumber = page,
                PageSize = PageSize,
                TotalItems = total,
                Status = status,
                AllCount = allCount,
                OngoingCountAll = ongoingCount,
                CompletedCountAll = completedCount
            };

            return PartialView(
                "~/Views/Shared/Partials/Home/_ProjectView.cshtml", vm
            );
        }

        [HttpGet("LoadMembers")]
        public async Task<IActionResult> LoadMembers(
          string? term,
          int page = 1)
        {
            IEnumerable<MemberDto> dtos;
            int total;

            if (!string.IsNullOrWhiteSpace(term))
            {
                dtos = await _tagService.SearchMembersAsync(term);
                total = dtos.Count();
                page = 1;
            }
            else
            {
                total = await _memberService.CountAsync();
                dtos = await _memberService.GetPagedAsync(
                          (page - 1) * PageSize,
                          PageSize);
            }

            var vm = new MemberIndex
            {
                Items = dtos,
                PageNumber = page,
                PageSize = PageSize,
                TotalItems = total
            };
            return PartialView(
              "~/Views/Shared/Partials/Home/_MemberView.cshtml",
              vm
            );
        }
    }
}
