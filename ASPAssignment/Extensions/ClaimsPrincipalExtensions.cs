using System.Security.Claims;

namespace ASPAssignment.Extensions;
//claimsprincipal extension för att kolla om en användare är admin
public static class ClaimsPrincipalExtensions
{
    public static bool IsAppAdmin(this ClaimsPrincipal user)
    {
        return user.HasClaim(c => c.Type == "IsAppAdmin" && c.Value == "true");
    }
}
