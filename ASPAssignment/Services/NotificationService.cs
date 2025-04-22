using ASPAssignment.Hubs;
using Business.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace ASPAssignment.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationService(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public Task SendNotificationAsync(string userId, NotificationDto notification)
    {
        return _hub.Clients.User(userId)
                   .SendAsync("ReceiveNotification", notification);
    }
}
