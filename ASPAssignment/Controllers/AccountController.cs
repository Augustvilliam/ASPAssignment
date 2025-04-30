
using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASPAssignment.Services;

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
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = string.Empty;
                return View(form);
            }

            var dto = new LoginDto
            {
                Email = form.Email,
                Password = form.Password
            };

            if (!await _accountService.LoginAsync(dto))
            {
                var err = "Invalid login attempt.";
                ModelState.AddModelError(string.Empty, err);
                ViewBag.ErrorMessage = err;
                return View(form);
            }

            // Skicka profilpåminnelse om profil ej är komplett:
            var user = await _memberService.GetMemberByEmailAsync(form.Email);
            if (user != null && !user.HasCompleteProfile)
            {
                var reminder = new NotificationDto
                {
                    ImageUrl = user.ProfileImageUrl ?? "/img/default-user.svg",
                    Message = "Komplettera din profil med bild, telefon och adress.",
                    Timestamp = DateTime.UtcNow,
                    NotificationId = Guid.NewGuid().ToString(),
                    NotificationType = "ProfileReminder"
                };
                await _notificationService.SendNotificationAsync(user.Email, reminder);
            }

            return LocalRedirect(returnUrl);
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
                return View(form);

            var dto = new RegisterDto
            {
                FirstName = form.FirstName,
                LastName = form.LastName,
                Email = form.Email,
                Password = form.Password
            };

            var result = await _accountService.RegisterAsync(dto);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(form);
            }

            // Lyckad registrering – skicka global notis
            var displayName =
                !string.IsNullOrWhiteSpace(form.FirstName) && !string.IsNullOrWhiteSpace(form.LastName)
                ? $"{form.FirstName} {form.LastName}"
                : form.Email;

            var joinedNotification = new NotificationDto
            {
                ImageUrl = "/img/default-user.svg",
                Message = $"{displayName} Has joined, Say hi!",
                Timestamp = DateTime.UtcNow,
                NotificationType = "UserJoined"
                // NotificationId fylls av SendNotificationAsync per användare
            };

            // Hämta alla medlemmar och skicka notis var för sig:
            var allMembers = await _memberService.GetAllMembersAsync();
            foreach (var member in allMembers)
            {
                await _notificationService.SendNotificationAsync(member.Email, joinedNotification);
            }

            return LocalRedirect("~/Account/Login");
        }
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await _accountService.SignOutAsync();
            return LocalRedirect("~/");
        }
    }
}
