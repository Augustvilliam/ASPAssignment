
using Business.Dtos;
using Business.Factories;
using Business.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class MemberService(UserManager<MemberEntity> userManager) : IMemberService
{
    private readonly UserManager<MemberEntity> _userManager = userManager;

    public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
    {
        var list = await _userManager.Users.ToListAsync();
        return list.Select(MemberFactory.FromEntity);

    }
    public async Task<MemberDto> GetMemberByIdAsync(string id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user == null ? null : MemberFactory.FromEntity(user);
    }

    public async Task<bool> UpdateMemberAsync(MemberDto dto, string? imagePath)
    {
        var user = await _userManager.FindByIdAsync(dto.Id);
        if (user == null) return false;

        MemberFactory.UpdateEntity(user, dto);

        if (!string.IsNullOrEmpty(imagePath))
        {
            user.ProfileImagePath = imagePath;
        }

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteMemberAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;

        var resualt = await _userManager.DeleteAsync(user);
        return resualt.Succeeded;
    }

    public async Task<MemberDto?> GetMemberForUpdateAsync(string id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        return user == null ? null : MemberFactory.FromEntity(user);
    }
}



