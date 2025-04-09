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
        public AdminController(SignInManager<MemberEntity> signInManager, UserManager<MemberEntity> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
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
    }
}
