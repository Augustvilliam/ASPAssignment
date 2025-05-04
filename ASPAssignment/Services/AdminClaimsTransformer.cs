using Data.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace ASPAssignment.Services;
//claimstransformer för att lägga till en claim i användarens identitet
public class AdminClaimsTransformer : IClaimsTransformation 
{
    private readonly UserManager<MemberEntity> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AdminClaimsTransformer(
        UserManager<MemberEntity> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(c => c.Type == "IsAppAdmin"))
            return principal;

        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null)
            return principal;

        var user = await _userManager.GetUserAsync(principal);
        if (user == null)
            return principal;

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null && role.IsAdmin)
            {
                identity.AddClaim(new Claim("IsAppAdmin", "true"));
                break;
            }
        }

        return principal;
    }
}
