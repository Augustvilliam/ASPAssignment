using Business.Dtos;
using Data.Entities;

namespace Business.Factories;

public static class ProjectFactory
{
    public static ProjectEntity CreateEntity(ProjectDto dto, IEnumerable<MemberEntity> members)
    {
        return new ProjectEntity
        {
            Id = dto.Id,
            ProjectName = dto.ProjectName,
            ClientName = dto.ClientName,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Budget = dto.Budget,
            ProjectImagePath = dto.ProjectImagePath,
            Members = members.ToList(),
            Status = dto.Status
        };
    }

    public static ProjectDto FromEntity(ProjectEntity entity)
    {
        return new ProjectDto
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            ClientName = entity.ClientName,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget,
            ProjectImagePath = entity.ProjectImagePath,
            MemberIds = entity.Members.Select(m => m.Id).ToList(),
            Status = entity.Status,
            Members = entity.Members?
                        .Select(MemberFactory.FromEntity)
                        .ToList()
                    ?? new List<MemberDto>()
        };
    }
    public static void UpdateEntity(ProjectEntity entity, ProjectDto dto, IEnumerable<MemberEntity> members)
    {
        entity.ProjectName = dto.ProjectName;
        entity.ClientName = dto.ClientName;
        entity.Description = dto.Description;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.Budget = dto.Budget;
        entity.Status = dto.Status;

        if (!string.IsNullOrWhiteSpace(dto.ProjectImagePath))
            entity.ProjectImagePath = dto.ProjectImagePath;

        entity.Members = new List<MemberEntity>(members);

    }

}