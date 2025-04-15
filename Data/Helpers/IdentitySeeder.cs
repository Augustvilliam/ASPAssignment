

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Helpers;

public class IdentitySeeder
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<MemberEntity>>();

        string[] roles = { "Admin", "User" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "admin@admin.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var newAdminUser = new MemberEntity
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                Profile = new MemberProfileEntity
                {
                    FirstName = "Admin",
                    LastName = "User"
                }
            };

            var result = await userManager.CreateAsync(newAdminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, "Admin");
            }
        }
    }
}
