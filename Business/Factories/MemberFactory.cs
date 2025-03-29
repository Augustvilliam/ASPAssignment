

using Business.Dtos;
using Data.Entities;

namespace Business.Factories;

public static class MemberFactory
{
    public static MemberDto FromEntity(MemberEntity entity)
    {
        return new MemberDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName ?? string.Empty,
            LastName = entity.LastName ?? string.Empty,
            Email = entity.Email ?? string.Empty,
            Phone = entity.PhoneNumber,
            ProfileImagePath = entity.ProfileImagePath
        };
    }
    public static void UpdateEntity(MemberEntity entity, MemberDto dto)
    {
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.PhoneNumber = dto.Phone;
        entity.JobTitle = dto.JobTitle;
        entity.ProfileImagePath = dto.ProfileImagePath;
    }
}