using Business.Dtos;

namespace ASPAssignment.Services;

public interface INotificationService
{
    Task BroadcastNotificationAsync(NotificationDto notification);
    Task ClearAllForUserAsync(string userId);
    Task DismissAsync(Guid notificationId, string userId);
    Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(string userId);
    Task SendNotificationAsync(string userId, NotificationDto notification);
}