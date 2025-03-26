using Business.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

[Route("Member")]
public class MemberController : Controller
{
    private readonly IMemberService _memberService;

    public MemberController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet("GetMember/{id}")]
    public async Task<IActionResult> GetMember(string id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound();

        return Json(member);
    }

    [HttpPost("Update")]
    public async Task<IActionResult> UpdateMember(MemberUpdateForm form)
    {
        if (ModelState.IsValid)
        {
            var result = await _memberService.UpdateMemberAsync(form);
            if (result)
                return RedirectToAction("LoadTeamMembers", "Navigation");
        }
        return BadRequest("Could Not Update Member");
    }

}
