using System.Security.Claims;
using Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPAssignment.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ChatController : Controller
{
    private readonly DataContext _context;
    public ChatController(DataContext context) => _context = context;

    [HttpGet("History")]
    public async Task<IActionResult> History(string otherUserId) //laddar in historik till en användare
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(otherUserId))
            return BadRequest();

        var history = await (
            from m in _context.ChatMessages
            where (m.SenderId == currentUserId && m.RecipientId == otherUserId)
               || (m.SenderId == otherUserId && m.RecipientId == currentUserId)
            join u in _context.Users.Include(u => u.Profile) on m.SenderId equals u.Id
            orderby m.Timestamp
            select new
            {
                senderId = m.SenderId,
                senderName = u.Profile != null
                               ? $"{u.Profile.FirstName} {u.Profile.LastName}"
                               : u.UserName,
                text = m.Text,
                timestamp = m.Timestamp
            }
        ).ToListAsync();

        var toMarkRead = await _context.ChatMessages
            .Where(m =>
                m.RecipientId == currentUserId &&
                m.SenderId == otherUserId &&
                !m.IsRead)
            .ToListAsync();

        if (toMarkRead.Any())
        {
            toMarkRead.ForEach(m => m.IsRead = true);
            await _context.SaveChangesAsync();
        }

        return Ok(history);
    }

    [HttpGet("UnreadCounts")]
    public async Task<IActionResult> UnreadCounts() //räknar olästa meddelande för den lilla röda pricken på varje användare i chatten om det finns. 
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var counts = await _context.ChatMessages
            .Where(m => m.RecipientId == userId && !m.IsRead)
            .GroupBy(m => m.SenderId)
            .Select(g => new
            {
                otherUserId = g.Key,
                unreadCount = g.Count()
            })
            .ToListAsync();

        return Ok(counts);
    }
}