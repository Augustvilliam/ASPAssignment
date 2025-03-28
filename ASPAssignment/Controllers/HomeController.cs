using System.Diagnostics;
using ASPAssignment.Models;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers
{
    public class HomeController : Controller

    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProjectService _projectService;
        public HomeController(ILogger<HomeController> logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }
        [Authorize]

        public async Task<IActionResult> Index()
        {
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
