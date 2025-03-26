using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interface;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class MemberService(UserManager<MemberEntity> userManager) : IMemberService
{
    private readonly UserManager<MemberEntity> _userManager = userManager;

    public async Task<IEnumerable<Member>> GetAllMembers()
    {
        var list = await _userManager.Users.ToListAsync();
        var members = list.Select(x => new Member
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Phone = x.PhoneNumber,
            JobTitle = x.JobTitle
        });
        return members;
    }

    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) return null;

        return new Member
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.PhoneNumber,
            JobTitle = user.JobTitle
        };
    }

    public async Task<bool> UpdateMemberAsync(MemberUpdateForm form)
    {   
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == form.Id);
        if (user == null) return false;

        user.FirstName = form.FirstName;
        user.LastName = form.LastName;
        user.JobTitle = form.JobTitle;
        user.PhoneNumber = form.Phone;

        if (form.ProfilePic != null &&  form.ProfilePic.Length > 0)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}_{form.ProfilePic.FileName}";
            var filePath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await form.ProfilePic.CopyToAsync(stream);

            user.ProfileImagePath = $"/uploads/{fileName}";
        }

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<bool> DeleteMemberAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<MemberUpdateForm?> GetMemberForUpdateAsync(string id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) return null;

        return new MemberUpdateForm
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            JobTitle = user.JobTitle,
            Email = user.Email,
            Phone = user.PhoneNumber,
            ExistingProfileImagePath = user.ProfileImagePath
        };
    }

}



