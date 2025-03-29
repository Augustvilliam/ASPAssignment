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
            Members = members.ToList()
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
            MemberIds = entity.Members.Select(m => m.Id).ToList()
        };
    }
}