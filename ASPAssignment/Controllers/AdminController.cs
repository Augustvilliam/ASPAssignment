using System.Reflection.Metadata.Ecma335;
using ASPAssignment.ViewModels;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers
{
    public class AdminController : Controller
    {
        private readonly SignInManager<MemberEntity> _signInManager;
        private readonly UserManager<MemberEntity> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AdminController(SignInManager<MemberEntity> signInManager,
            UserManager<MemberEntity> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginForm form)
        {
            if (!ModelState.IsValid)
                return View(form);
            var user = await _userManager.FindByEmailAsync(form.Email);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ViewBag.ErrorMessage = "Invadlid Login Attempt, or not an admin";
                return View(form);
            }
            var reuslt = await _signInManager.PasswordSignInAsync(user, form.Password, form.RememberMe, false);
            if (reuslt.Succeeded)
                return RedirectToAction("Index", "Home");

            ViewBag.ErrorMessage = "Somethign went wrong, please try again";
            return View(form);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create() => View(new CreateRole());

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateRole model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if(await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("", "Role already exists");
                return View(model);
            }
            var role = new ApplicationRole
            {
                Name = model.Name,
                IsAdmin = model.IsAdmin
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            if (role.Name == "User" || role.Name == "Admin")
                return Forbid();

            await _roleManager.DeleteAsync(role);
            return RedirectToAction(nameof(Index));
        }
    }
}
