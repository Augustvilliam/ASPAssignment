using ASPAssignment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace ASPAssignment.Controllers
{
    public class SettingsController(UserManager<MemberEntity> userManager) : Controller
    {
        private readonly UserManager<MemberEntity> _userManager = userManager;


        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = user?.Profile;

            var model = new SettingsFormViewModel
            {
                Id = user?.Id,
                FirstName = profile?.FirstName ?? "",
                LastName = profile?.LastName ?? "",
                Phone = user?.PhoneNumber ?? "",
                Email = user?.Email ?? "",
                StreetAddress = profile?.StreetAddress ?? "",
                PostalCode = profile?.PostalCode ?? "",
                City = profile?.City ?? "",
                ExistingProfileImagePath = user?.ProfileImagePath
            };
            ViewBag.ProfileImage = user?.ProfileImagePath ?? "/img/Employee.svg";
            ViewBag.Email = user?.Email ?? "";
            ViewBag.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User";
            ViewBag.FullName = profile != null
                ? $"{profile.FirstName} {profile.LastName}".Trim()
                : "User";

            return View(model);
        }
    }
}
