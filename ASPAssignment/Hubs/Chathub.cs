using System.Security.Claims;
using Data.Contexts;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ASPAssignment.Hubs
{
    public class Chathub(DataContext context) : Hub
    {
        private readonly DataContext _context = context;
        private static readonly Dictionary<string, string> _userConnections = new();

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
        public async Task SendMessage(string message)
        {
            var userId = Context.UserIdentifier;
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            string senderName = user?.Profile != null
                ? $"{user.Profile.FirstName} {user.Profile.LastName}"
                : "Unknown";
            await Clients.All.SendAsync("ReceiveMessage", senderName, message);
        }

        public async Task SendPrivateMEssage(string recipientId, string message)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(senderId == null)
            {
                await Clients.Caller.SendAsync("ReciveMessage", "System", "Missing User");
                return;
            }

            var sender = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == senderId);
            string senderName = sender?.Profile != null
                    ? $"{sender.Profile.FirstName} {sender.Profile.LastName}"
                    :"Unknown";

            if (_userConnections.TryGetValue(recipientId, out var recipientConnectionId))
            {
                await Clients.Client(recipientConnectionId).SendAsync("ReceivePrivateMessage", senderName, message);
            }

            if (_userConnections.TryGetValue(senderId, out var senderConnectionId))
            {
                await Clients.Client(senderConnectionId).SendAsync("ReceivePrivateMessage", senderName, message);
            }

        }
    }
}
