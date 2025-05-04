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
        dto.HasCompleteProfile = //restkod från när jag försäkte implementera automatiska notiser när en profil inte var komplett. men fick det aldrig fungera så nu ligger den bara kvar så inget går sönder. 
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
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var user = await _userManager.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == dto.Id);
            if (user == null) return false;

            if (user.Profile == null)
            {
                user.Profile = new MemberProfileEntity { MemberId = user.Id };
                _context.MemberProfile.Add(user.Profile);
            }

            // --- hitta/validera targetRole som tidigare ---
            var targetRole = !string.IsNullOrWhiteSpace(dto.RoleId)
                ? await _roleManager.FindByIdAsync(dto.RoleId)
                : null;
            if (targetRole == null)
            {
                targetRole = await _roleManager.FindByNameAsync("User")
                            ?? throw new InvalidOperationException("Standardrollen saknas!");
            }

            //  --- räddad av chatgpt eftersom man kunde delete tillsatta roller som satt användaren i limbo. 
            user.Profile.RoleId = targetRole.Id;

            //Identity‐roller på Membership‐tabellen
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(targetRole.Name!))
            {
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, targetRole.Name!);
            }

            //övriga fält
            MemberFactory.UpdateEntity(user, dto);
            if (!string.IsNullOrEmpty(imagePath))
                user.ProfileImagePath = imagePath;

            //Spara Identity‐delen
            var idRes = await _userManager.UpdateAsync(user);
            if (!idRes.Succeeded)
            {
                await tx.RollbackAsync();
                return false;
            }

            //Spara EF‐delen (inklusive Profile.RoleId)
            await _context.SaveChangesAsync();

            await tx.CommitAsync();
            return true;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
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

    public async Task<int> CountAsync() // räknar antalet användare för paginering
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
