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
    public async Task ClearAllForUserAsync(string userId)
    {
        var notes = _db.Notifications.Where(n => n.UserId == userId);
        _db.Notifications.RemoveRange(notes);
        _db.SaveChanges();
        await _db.SaveChangesAsync();
    }
    public async Task BroadcastNotificationAsync(NotificationDto notification)
    {
        // 1) Hämta alla userIds från Identity-tabellen
        var userIds = await _db.Users
                               .Select(u => u.Id)
                               .ToListAsync();

        // 2) Bygg upp NotificationEntity-objekt och motsvarande DTO per user
        var entities = new List<NotificationEntity>(userIds.Count);
        var userNotifications = new Dictionary<string, NotificationDto>(userIds.Count);

        foreach (var uid in userIds)
        {
            var id = Guid.NewGuid();
            // Spara entity för just den användaren
            entities.Add(new NotificationEntity
            {
                Id = id,
                UserId = uid,
                Message = notification.Message,
                ImageUrl = notification.ImageUrl,
                Timestamp = notification.Timestamp,
                NotificationType = notification.NotificationType
            });

            // Klona DTO med rätt NotificationId
            userNotifications[uid] = new NotificationDto
            {
                NotificationId = id.ToString(),
                Message = notification.Message,
                ImageUrl = notification.ImageUrl,
                Timestamp = notification.Timestamp,
                NotificationType = notification.NotificationType
            };
        }

        // 3) Spara alla notiser i ett svep
        await _db.Notifications.AddRangeAsync(entities);
        await _db.SaveChangesAsync();

        // 4) Skicka en SignalR-händelse till var och en
        foreach (var uid in userIds)
        {
            var dto = userNotifications[uid];
            await _hub.Clients.User(uid)
                      .SendAsync("ReceiveNotification", dto);
        }
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
