using ASPAssignment.Services;
using Business.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notisSvc;

        public NotificationsController(INotificationService notisSvc)
            => _notisSvc = notisSvc;

        [HttpGet]
        public async Task<IEnumerable<NotificationDto>> Get()
        {
            var list = await _notisSvc.GetNotificationsForUserAsync(User.Identity.Name!);
            // Om list är null, returnera en tom lista istället:
            return list ?? new List<NotificationDto>();
        }

        [HttpPost("dismiss/{id:guid}")]
        public async Task<IActionResult> Dismiss(Guid id)
        {
            await _notisSvc.DismissAsync(id, User.Identity.Name!);
            return Ok();
        }
    }
}
