using Business.Dtos;
using Data.Entities;

namespace Business.Factories;

public static class MemberFactory
{
    public static MemberDto FromEntity(MemberEntity entity)
    {
        var dto = new MemberDto
        {
            Id = entity.Id,
            FirstName = entity.Profile?.FirstName ?? string.Empty,
            LastName = entity.Profile?.LastName ?? string.Empty,
            Email = entity.Email ?? string.Empty,
            Phone = entity.PhoneNumber,
            RoleId = entity.Profile?.RoleId ?? string.Empty,
            JobTitle = entity.Profile?.Role?.Name ?? string.Empty,
            BirthDate = entity.Profile?.BirthDate,
            StreetAddress = entity.Profile?.StreetAddress,
            City = entity.Profile?.City,
            PostalCode = entity.Profile?.PostalCode,
            ProfileImagePath = entity.ProfileImagePath
        };

        // lite restkod från när jag försökte få till så att notiserna skickades dynamiskt ifall inormation saknades. fick det aldrig att funka och är nu för rädd att ta bort det och ta sönder någont 04/05
        dto.HasCompleteProfile =
            !string.IsNullOrWhiteSpace(dto.ProfileImagePath) &&
            !string.IsNullOrWhiteSpace(dto.Phone) &&
            !string.IsNullOrWhiteSpace(dto.StreetAddress) &&
            !string.IsNullOrWhiteSpace(dto.City) &&
            !string.IsNullOrWhiteSpace(dto.PostalCode) &&
            !string.IsNullOrWhiteSpace(dto.RoleId);

        return dto;
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
        entity.Profile.BirthDate = dto.BirthDate;
        entity.Profile.StreetAddress = dto.StreetAddress;
        entity.Profile.City = dto.City;
        entity.Profile.PostalCode = dto.PostalCode;

        // rollid från dto
        if (!string.IsNullOrWhiteSpace(dto.RoleId))
        {
            entity.Profile.RoleId = dto.RoleId;
        }
    }
}
