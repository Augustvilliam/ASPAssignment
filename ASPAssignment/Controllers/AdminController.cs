using ASPAssignment.ViewModels;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers
{
    [Authorize(Policy = "RequireAppAdmin")]
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
        public async Task<IActionResult> Login(AdminLoginForm form, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please enter valid information.";
                return View(form);
            }

            //Först: Prova vanlig inloggning
            var signInResult = await _signInManager
                .PasswordSignInAsync(form.Email, form.Password, form.RememberMe, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                ViewBag.ErrorMessage = "Invalid Email Or password.";
                return View(form);
            }

            //Hämta användaren och kontrollera IsAdmin-flaggan på varje roll
            var user = await _userManager.FindByEmailAsync(form.Email);
            var roles = await _userManager.GetRolesAsync(user!);
            var isAdmin = false;
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role?.IsAdmin == true)
                {
                    isAdmin = true;
                    break;
                }
            }

            if (!isAdmin)
            {
                // Logga ut direkt om det inte är en admin
                await _signInManager.SignOutAsync();
                ViewBag.ErrorMessage = "Invalid Authorization, please use the Member-portal.";
                return View(form);
            }

            //Inloggad som admin – skicka vidare
            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateRole());

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

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var vm = new EditRole
            {
                Id = role.Id,
                Name = role.Name,
                IsAdmin = role.IsAdmin
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRole model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null) return NotFound();

            // (Valfritt) hindra att standardroller byter namn
            if ((role.Name == "User" || role.Name == "Admin") && role.Name != model.Name)
            {
                ModelState.AddModelError("", "Du kan inte byta namn på standardrollerna.");
                return View(model);
            }

            role.Name = model.Name;
            role.IsAdmin = model.IsAdmin;

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);

            return View(model);
        }
    }
}

