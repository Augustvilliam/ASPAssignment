using Business.Dtos;
using Business.Factories;
using Business.Interface;
using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class MemberService: IMemberService
{
    private readonly UserManager<MemberEntity> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly DataContext _context;

    public MemberService(
        UserManager<MemberEntity> userManager,
        RoleManager<ApplicationRole> roleManager,
        DataContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
    {
        var list = await _userManager.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Role)
            .ToListAsync();

        return list.Select(MemberFactory.FromEntity);
    }

    public async Task<MemberDto?> GetMemberByIdAsync(string id)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Role)
            .FirstOrDefaultAsync(x => x.Id == id);

        return user == null ? null : MemberFactory.FromEntity(user);
    }

    public async Task<MemberDto?> GetMemberByEmailAsync(string email)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return null;

        var dto = MemberFactory.FromEntity(user);
        dto.ProfileImageUrl = user.ProfileImagePath;
        dto.HasCompleteProfile =
            !string.IsNullOrEmpty(user.ProfileImagePath) &&
            !string.IsNullOrEmpty(user.PhoneNumber) &&
            !string.IsNullOrEmpty(user.Profile?.StreetAddress) &&
            !string.IsNullOrEmpty(user.Profile?.City) &&
            !string.IsNullOrEmpty(user.Profile?.PostalCode);

        return dto;
    }

    public async Task<List<MemberDto>> GetAllAdminsAsync()
    {
        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
        // Include profile and role for DTO
        var list = new List<MemberDto>();
        foreach (var user in adminUsers)
        {
            var u = await _userManager.Users
                .Include(x => x.Profile)
                    .ThenInclude(p => p.Role)
                .FirstAsync(x => x.Id == user.Id);
            var dto = MemberFactory.FromEntity(u);
            dto.ProfileImageUrl = u.ProfileImagePath ?? "/img/default-user.svg";
            list.Add(dto);
        }
        return list;
    }

    public async Task<MemberDto?> GetMemberForUpdateAsync(string id)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Role)
            .FirstOrDefaultAsync(x => x.Id == id);

        return user == null ? null : MemberFactory.FromEntity(user);
    }

    public async Task<bool> UpdateMemberAsync(MemberDto dto, string? imagePath)
    {
        // 1) Hämta användare inkl. profil
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (user == null)
            return false;

        // 2) Se till att en profil alltid finns
        if (user.Profile == null)
        {
            user.Profile = new MemberProfileEntity { MemberId = user.Id };
            _context.MemberProfile.Add(user.Profile);
        }

        // 3) Uppdatera roller **ENDAST om** RoleId skiljer sig
        var currentRoles = await _userManager.GetRolesAsync(user);
        var newRoleEntity = await _roleManager.FindByIdAsync(dto.RoleId);
        if (newRoleEntity != null)
        {
            var newRoleName = newRoleEntity.Name!;
            if (!currentRoles.Contains(newRoleName))
            {
                // Ta bort gamla roller
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Tilldela ny
                await _userManager.AddToRoleAsync(user, newRoleName);
            }
        }

        // 4) Använd din factory för att uppdatera profilfält
        MemberFactory.UpdateEntity(user, dto);

        // 5) Bild och telefonnummer (factory uppdaterar ProfileImagePath och PhoneNumber)
        if (!string.IsNullOrEmpty(imagePath))
            user.ProfileImagePath = imagePath;

        // 6) Spara user (inbegriper AspNetUsers-tabellen och UserRoles)
        var userUpdateResult = await _userManager.UpdateAsync(user);
        if (!userUpdateResult.Succeeded)
            return false;

        // 7) Spara MemberProfile-ändringar (BirthDate, Address, RoleId etc.)
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> DeleteMemberAsync(string id)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Role)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<int> CountAsync()
    {
        return await _userManager.Users.CountAsync();
    }

    public async Task<IEnumerable<MemberDto>> GetPagedAsync(int skip, int take)
    {
        var list = await _userManager.Users
            .Include(u => u.Profile)
                .ThenInclude(p => p.Role)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return list.Select(MemberFactory.FromEntity);
    }
}
