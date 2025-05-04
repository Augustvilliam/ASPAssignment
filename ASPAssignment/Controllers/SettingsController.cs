using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ASPAssignment.Controllers;
[Authorize]
public class SettingsController(
    UserManager<MemberEntity> userManager,
    SignInManager<MemberEntity> signInManager,
    IMemberService memberService, RoleManager<ApplicationRole> roleManager) : Controller
{
    private readonly UserManager<MemberEntity> _userManager = userManager;
    private readonly SignInManager<MemberEntity> _signInManager = signInManager;
    private readonly IMemberService _memberService = memberService;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;


    public async Task<IActionResult> Settings()
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

        if (user == null) return NotFound();

        if (user.Profile == null)
        {
            user.Profile = new MemberProfileEntity
            {
                MemberId = user.Id,
                RoleId = (await _roleManager.FindByNameAsync("User"))?.Id ?? ""
            };
            // (du behöver inte spara permanent här, bara för viewmodellen)
        }

        var profile = user.Profile!;
        var model = new SettingsFormViewModel
        {
            Id = user.Id,
            RoleId = profile.RoleId,
            DateOfBirth = profile.BirthDate,
            FirstName = profile.FirstName ?? "",
            LastName = profile.LastName ?? "",
            Phone = user.PhoneNumber,
            Email = user.Email,
            StreetAddress = profile.StreetAddress,
            PostalCode = profile.PostalCode,
            City = profile.City,
            ExistingProfileImagePath = user.ProfileImagePath
        };

        ViewBag.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "N/A";
        ViewBag.ProfileImage = user.ProfileImagePath ?? "/img/Employee.svg";
        ViewBag.Email = user.Email;

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> LoadSection(string section)
    {
        if (section == "User")
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

            return PartialView("Partials/Settings/_SettingsForm", model);
        }

        return section switch
        {
            "Privacy" => PartialView("Partials/Settings/_Privacy"),
            "Preferences" => PartialView("Partials/Settings/_Preferences"),
            "Terms" => PartialView("Partials/Settings/_Terms"),
            _ => PartialView("Partials/Settings/_Privacy"), // fallback
        };
    }

    [Authorize]
    [HttpPost] 
    public async Task<IActionResult> DeleteAccount() //just nu kan bara användare ta bort sina egna konton. admins kan inte ta bort någon annan. 
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);
        }

        return RedirectToAction("Login", "Account");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Update(SettingsFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Settings", model);
        }

        string? imagePath = null;
        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ProfileImage.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await model.ProfileImage.CopyToAsync(stream);

            imagePath = $"/uploads/{fileName}";
        }

        // Skapa DTO och skicka till service
        var dto = new MemberDto
        {
            Id = model.Id,
            RoleId = model.RoleId,
            BirthDate = model.DateOfBirth,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            StreetAddress = model.StreetAddress,
            PostalCode = model.PostalCode,
            City = model.City,
            ProfileImagePath = imagePath ?? model.ExistingProfileImagePath
        };

        var result = await _memberService.UpdateMemberAsync(dto, imagePath);
        if (!result)
        {
            ViewBag.ErrorMessage = "Failed to update profile.";
            return View("Settings", model);
        }

        return RedirectToAction("Settings");
    }

}

