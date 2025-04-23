using System;
using System.Threading.Tasks;
using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASPAssignment.Services;
using ASPAssignment.Hubs;

namespace ASPAssignment.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMemberService _memberService;
        private readonly INotificationService _notificationService;

        public AccountController(
            IAccountService accountService,
            IMemberService memberService,
            INotificationService notificationService)
        {
            _accountService = accountService;
            _memberService = memberService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "~/")
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ErrorMessage = string.Empty;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginForm form, string returnUrl = "/Home/Index")
        {
            if (ModelState.IsValid)
            {
                var dto = new LoginDto
                {
                    Email = form.Email,
                    Password = form.Password
                };

                var result = await _accountService.LoginAsync(dto);
                if (result)
                {
                    // Skicka profilpåminnelse om profil ej är komplett
                    var user = await _memberService.GetMemberByEmailAsync(form.Email);
                    if (user != null && !user.HasCompleteProfile)
                    {
                        var notification = new NotificationDto
                        {
                            ImageUrl = user.ProfileImageUrl ?? "/img/default-user.svg",
                            Message = "Komplettera din profil med bild, telefon och adress.",
                            Timestamp = DateTime.UtcNow,
                            NotificationId = Guid.NewGuid().ToString(),
                            NotificationType = "ProfileReminder"
                        };
                        await _notificationService.SendNotificationAsync(user.Email, notification);
                    }

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorMessage = "Invalid login attempt.";
            }

            return View(form);
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.ErrorMessage = string.Empty;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterForm form)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "One or more fields are invalid.";
                return View(form);
            }

            var dto = new RegisterDto
            {
                FirstName = form.FirstName,
                LastName = form.LastName,
                Email = form.Email,
                Password = form.Password
            };

            var result = await _accountService.RegisterAsync(dto);

            if (result.Succeeded)
            {
                var name = !string.IsNullOrEmpty(form.FirstName) && !string.IsNullOrEmpty(form.LastName)
                    ? $"{form.FirstName} {form.LastName}"
                    : form.Email;

                var notification = new NotificationDto
                {
                    ImageUrl = "/img/default-user.svg",
                    Message = $"{name} har registrerat sig! Hälsa hen välkomna i chatten!",
                    Timestamp = DateTime.UtcNow,
                    NotificationId = Guid.NewGuid().ToString(),
                    NotificationType = "UserJoined"
                };
                await _notificationService.BroadcastNotificationAsync(notification);
                return RedirectToAction("Login", "Account");
            }
            // Om registreringen misslyckas, lägg till felmeddelanden i ModelState
            ViewBag.ErrorMessage = "Registration failed. Please try again.";
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(form);
        }
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await _accountService.SignOutAsync();
            return LocalRedirect("~/");
        }
    }
}
