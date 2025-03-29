using ASPAssignment.Models;
using Business.Interface;
using Domain.Models;
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
            var result = await _memberService.UpdateMemberAsync(form);
            if (result)
                return RedirectToAction("Index", "Home");
        }
        return BadRequest("Could Not Update Member");
    }

}
