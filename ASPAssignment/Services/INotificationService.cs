using Business.Dtos;

namespace ASPAssignment.Services;

public interface INotificationService
{
    Task SendNotificationAsync(string userId, NotificationDto notification);
}