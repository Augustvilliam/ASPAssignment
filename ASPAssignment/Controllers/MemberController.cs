using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

[Route("Member")]
[Authorize]
public class MemberController : Controller
{
    private readonly IMemberService _memberService;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public MemberController(
        IMemberService memberService,
        RoleManager<ApplicationRole> roleManager)
    {
        _memberService = memberService;
        _roleManager = roleManager;
    }


    [Authorize]
    [HttpGet("GetMember/{id}")]
    public async Task<IActionResult> GetMember(string id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound();

        // Hämta alla roller för dropdown på klienten
        var roles = _roleManager.Roles
            .Select(r => new { r.Id, r.Name })
            .ToList();

        return Json(new
        {
            member,
            roles
        });
    }

    [Authorize]
    [HttpPost("Update")]
    public async Task<IActionResult> UpdateMember(MemberUpdateForm form)
    {
        if (!ModelState.IsValid)
        {
            // Valideringsfel
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });
            return BadRequest(errors);
        }

        // antera uppladdad bild
        string? imagePath = form.ExistingProfileImagePath;
        if (form.ProfilePic != null && form.ProfilePic.Length > 0)
        {
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var fileName = $"{Guid.NewGuid()}_{form.ProfilePic.FileName}";
            var fullPath = Path.Combine(uploadDir, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await form.ProfilePic.CopyToAsync(stream);

            imagePath = $"/uploads/{fileName}";
        }

        // Bygg DTO
        var dto = new MemberDto
        {
            Id = form.Id,
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            Phone = form.Phone,
            RoleId = form.RoleId,                         
            ProfileImagePath = imagePath,
            StreetAddress = form.StreetAddress,
            City = form.City,
            PostalCode = form.PostalCode,
            BirthDate = form.BirthDate
        };

        //Skicka vidare till service
        var result = await _memberService.UpdateMemberAsync(dto, imagePath);
        if (result)
            return Json(new { success = true });

        return Json(new { success = false });
    }


    [HttpGet("Search")]
    [Route("Member/Search")]
    public async Task<IActionResult> Search(string? term = null)
    {
        var members = await _memberService.GetAllMembersAsync();
        var result = members.Select(m => new
        {
            id = m.Id,
            fullName = $"{m.FirstName} {m.LastName}",
            avatarUrl = Url.Content(m.ProfileImagePath ?? "/images/default-avatar.png")
        });
        return Json(result);
    }
}
