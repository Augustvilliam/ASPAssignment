using ASPAssignment.Hubs;
using Business.Dtos;
using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ASPAssignment.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;
    private readonly DataContext _db;

    public NotificationService(
        IHubContext<NotificationHub> hub,
        DataContext db)
    {
        _hub = hub;
        _db = db;
    }

    public async Task SendNotificationAsync(string userId, NotificationDto dto)
    {
        // 1) Persist
        var entity = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Message = dto.Message,
            ImageUrl = dto.ImageUrl,
            Timestamp = dto.Timestamp,
            NotificationType = dto.NotificationType
        };
        _db.Notifications.Add(entity);
        await _db.SaveChangesAsync();

        // 2) Push via SignalR (inkludera id så klienten vet vilken att dismiss:a)
        dto.NotificationId = entity.Id.ToString();
        await _hub.Clients.User(userId)
                 .SendAsync("ReceiveNotification", dto);
    }

    public async Task DismissAsync(Guid notificationId, string userId)
    {
        var entity = await _db.Notifications.FindAsync(notificationId);
        if (entity != null && entity.UserId == userId)
        {
            _db.Notifications.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
    public Task BroadcastNotificationAsync(NotificationDto notification)
    {
        return _hub.Clients.All.SendAsync("ReceiveNotification", notification);
    }
    public async Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(string userId)
    {
        var ents = await _db.Notifications
                           .Where(n => n.UserId == userId)
                           .OrderByDescending(n => n.Timestamp)
                           .ToListAsync();

        return ents.Select(e => new NotificationDto
        {
            NotificationId = e.Id.ToString(),
            Message = e.Message,
            ImageUrl = e.ImageUrl,
            Timestamp = e.Timestamp,
            NotificationType = e.NotificationType
        });
    }
}
