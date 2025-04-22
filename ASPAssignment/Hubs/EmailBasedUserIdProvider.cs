using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace ASPAssignment.Hubs;

public class EmailBasedUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
            => connection.User?.FindFirst(ClaimTypes.Email)?.Value;
}
