using System.Security.Claims;
using Data.Contexts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ASPAssignment.Hubs
{
    public class Chathub(DataContext context) : Hub
    {
        private readonly DataContext _context = context;
        private static readonly Dictionary<string, string> _userConnections = [];

        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
                _userConnections[userId] = Context.ConnectionId;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
                _userConnections.Remove(userId);

            return base.OnDisconnectedAsync(exception);
        }

        // Broadcast-meddelanden till alla
        public async Task SendMessage(string message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var senderName = user?.Profile != null
                ? $"{user.Profile.FirstName} {user.Profile.LastName}"
                : "Unknown";

            await Clients.All.SendAsync("ReceiveMessage", senderName, message, userId);
        }

        // Privata meddelanden
        public async Task SendPrivateMessage(string recipientId, string message)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (senderId == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "Kunde inte identifiera dig");
                return;
            }

            var sender = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == senderId);

            var senderName = sender?.Profile != null
                ? $"{sender.Profile.FirstName} {sender.Profile.LastName}"
                : "Unknown";

            // Skicka till mottagaren
            if (_userConnections.TryGetValue(recipientId, out var recConn))
            {
                await Clients.Client(recConn)
                             .SendAsync("ReceivePrivateMessage", senderName, message, senderId);
            }

            // Skicka tillbaka till avsändare så hen ser sitt egna meddelande
            if (_userConnections.TryGetValue(senderId, out var sndConn))
            {
                await Clients.Client(sndConn)
                             .SendAsync("ReceivePrivateMessage", senderName, message, senderId);
            }
        }
    }
}
