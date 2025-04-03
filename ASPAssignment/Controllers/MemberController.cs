using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

[Route("Member")]
public class MemberController(IMemberService memberService) : Controller
{
    private readonly IMemberService _memberService = memberService;

    [Authorize]
    [HttpGet("GetMember/{id}")]
    public async Task<IActionResult> GetMember(string id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound();

        return Json(member);
    }

    [Authorize]
    [HttpPost("Update")]
    public async Task<IActionResult> UpdateMember(MemberUpdateForm form)
    {
        if (ModelState.IsValid)
        {
            string? imagePath = null;
            if (form.ProfilePic != null && form.ProfilePic.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}_{form.ProfilePic.FileName}";
                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await form.ProfilePic.CopyToAsync(stream);

                imagePath = $"/uploads/{fileName}";
            }

            var dto = new MemberDto
            {
                Id = form.Id,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Email = form.Email,
                Phone = form.Phone,
                JobTitle = form.JobTitle,
                ProfileImagePath = imagePath ?? form.ExistingProfileImagePath // ✅ detta är nyckeln!
            };


            var result = await _memberService.UpdateMemberAsync(dto, imagePath);
            if (result)
            {
                // Skickar tillbaka en ny partialview eller JSON för uppdatering
                return Json(new { success = true });
            }
        }

        // Returnerar valideringsfel
        var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });

        return BadRequest(errors);
    }


}
