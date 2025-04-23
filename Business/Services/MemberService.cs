using Business.Dtos;
using Business.Factories;
using Business.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class MemberService(UserManager<MemberEntity> userManager) : IMemberService
{
    private readonly UserManager<MemberEntity> _userManager = userManager;

    public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
    {
        var list = await _userManager.Users
            .Include(u => u.Profile)
            .ToListAsync();

        return list.Select(MemberFactory.FromEntity);
    }

    public async Task<MemberDto?> GetMemberByIdAsync(string id)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(x => x.Id == id);

        return user == null ? null : MemberFactory.FromEntity(user);
    }
    public async Task<MemberDto?> GetMemberByEmailAsync(string email)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return null;

        var dto = MemberFactory.FromEntity(user);
        // Se till att ditt MemberDto innehåller dessa egenskaper:
        dto.ProfileImageUrl = user.ProfileImagePath;
        dto.HasCompleteProfile =
            !string.IsNullOrEmpty(user.ProfileImagePath)
            && !string.IsNullOrEmpty(user.PhoneNumber)
            && !string.IsNullOrEmpty(user.Profile?.StreetAddress)
            && !string.IsNullOrEmpty(user.Profile?.City)
            && !string.IsNullOrEmpty(user.Profile?.PostalCode);

        return dto;
    }
    public async Task<List<MemberDto>> GetAllAdminsAsync()
    {
        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

        return adminUsers.Select(user =>
        {
            var dto = MemberFactory.FromEntity(user);
            dto.ProfileImageUrl = user.ProfileImagePath ?? "/img/default-user.svg";
            return dto;
        }).ToList();
    }
    public async Task<MemberDto?> GetMemberForUpdateAsync(string id)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(x => x.Id == id);

        return user == null ? null : MemberFactory.FromEntity(user);
    }

    public async Task<bool> UpdateMemberAsync(MemberDto dto, string? imagePath)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (user == null)
            return false;

        // Skapa profil om den inte finns
        if (user.Profile == null)
        {
            user.Profile = new MemberProfileEntity
            {
                MemberId = user.Id
            };
        }

        MemberFactory.UpdateEntity(user, dto);

        if (!string.IsNullOrEmpty(imagePath))
            user.ProfileImagePath = imagePath;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteMemberAsync(string id)
    {
        var user = await _userManager.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
