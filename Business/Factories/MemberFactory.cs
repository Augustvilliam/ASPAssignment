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
            FirstName = entity.Profile?.FirstName ?? string.Empty,
            LastName = entity.Profile?.LastName ?? string.Empty,
            Email = entity.Email ?? string.Empty,
            Phone = entity.PhoneNumber,
            JobTitle = entity.Profile?.JobTitle,
            BirthDate = entity.Profile?.BirthDate,
            StreetAddress = entity.Profile?.StreetAddress,
            City = entity.Profile?.City,
            PostalCode = entity.Profile?.PostalCode,
            ProfileImagePath = entity.ProfileImagePath
        };
    }

    public static void UpdateEntity(MemberEntity entity, MemberDto dto)
    {
        if (entity.Profile == null)
        {
            entity.Profile = new MemberProfileEntity
            {
                MemberId = entity.Id
            };
        }

        entity.PhoneNumber = dto.Phone;

        if (!string.IsNullOrWhiteSpace(dto.ProfileImagePath))
        {
            entity.ProfileImagePath = dto.ProfileImagePath;
        }

        entity.Profile.FirstName = dto.FirstName;
        entity.Profile.LastName = dto.LastName;
        entity.Profile.JobTitle = dto.JobTitle;
        entity.Profile.BirthDate = dto.BirthDate;
        entity.Profile.StreetAddress = dto.StreetAddress;
        entity.Profile.City = dto.City;
        entity.Profile.PostalCode = dto.PostalCode;
    }
}