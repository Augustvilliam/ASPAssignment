using System.Security.Claims;

namespace ASPAssignment.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAppAdmin(this ClaimsPrincipal user)
    {
        return user.HasClaim(c => c.Type == "IsAppAdmin" && c.Value == "true");
    }
}
