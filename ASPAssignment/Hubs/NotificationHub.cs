using Business.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace ASPAssignment.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotification(string userId, NotificationDto notification)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", notification);
    }
    public async Task BroadcastNotification(NotificationDto notification)
    {
        await Clients.All.SendAsync("ReceiveNotification", notification);
    }
}
