using System.Security.Claims;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPAssignment.Controllers;

public class ChatController(DataContext context) : Controller
{
    private readonly DataContext _context = context;

    [HttpGet]
    public async Task<IActionResult> History (string otherUserId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(otherUserId))
            return BadRequest("");

        var history = await _context.ChatMessages
            .Where(m =>
            (m.SenderId == currentUserId && m.RecipientId == otherUserId) ||
            (m.SenderId == otherUserId && m.RecipientId == currentUserId)
            )
            .OrderBy(m => m.Timestamp)
            .Select(m => new
            {
                senderID = m.SenderId,
                text = m.Text,
                timestamp = m.Timestamp
            })
            .ToListAsync();

        return Json(history);
    }


}
