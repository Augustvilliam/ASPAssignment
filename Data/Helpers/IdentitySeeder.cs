﻿using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Helpers;

public class IdentitySeeder
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<MemberEntity>>();

        var roles = new[] //Seedar standardroller. ProjectLead som mellansteg mellan Admin och User
        {
            new ApplicationRole { Name = "Admin", IsAdmin = true },
            new ApplicationRole { Name = "User",  IsAdmin = false },
            new ApplicationRole { Name = "ProjectLead", IsAdmin = false }
        };

        foreach (var role in roles) //kollar om rollerna finns, annars skapas de
        {
            var existing = await roleManager.FindByNameAsync(role.Name);
            if (existing == null)
            {
                var createResult = await roleManager.CreateAsync(role);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                        Console.WriteLine($"[SeedRoles] Fel vid skapande av roll '{role.Name}': {error.Description}");
                }
            }
            else if (existing.IsAdmin != role.IsAdmin)
            {
                existing.IsAdmin = role.IsAdmin;
                var updateResult = await roleManager.UpdateAsync(existing);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        Console.WriteLine($"[SeedRoles] Fel vid uppdatering av roll '{role.Name}': {error.Description}");
                }
            }
        }

        // Hämta den seedade Admin-rollen och skapa en admin-användare om den inte finns
        var adminRole = await roleManager.FindByNameAsync("Admin");
        if (adminRole == null)
        {
            Console.WriteLine("[SeedRoles] Admin role not found after seeding.");
            return;
        }

        const string adminEmail = "admin@admin.com";
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
                    LastName = "User",
                    RoleId = adminRole.Id
                }
            };

            var createUserResult = await userManager.CreateAsync(newAdminUser, "Admin@123"); //bara något enkelt för att se att det funkar
            if (!createUserResult.Succeeded)
            {
                foreach (var error in createUserResult.Errors)
                    Console.WriteLine($"[SeedRoles] Fel vid skapande av admin-användare: {error.Description}");
            }
            else
            {
                var addToRoleResult = await userManager.AddToRoleAsync(newAdminUser, "Admin");
                if (!addToRoleResult.Succeeded)
                {
                    foreach (var error in addToRoleResult.Errors)
                        Console.WriteLine($"[SeedRoles] Fel vid tilldelning av Admin-roll: {error.Description}");
                }
            }
        }
    }
}
