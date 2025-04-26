using Data.Entities;
using Business.Dtos;

namespace Business.Interface
{
    public interface IProjectService
    {
        Task<int> CountAsync(string? status);
        Task CreateProjectAsync(ProjectDto dto);
        Task<bool> DeleteProjectAsync(Guid id);
        Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync();
        Task<IEnumerable<ProjectDto>> GetPagedAsync(string? status, int skip, int take);
        Task<ProjectDto?> GetProjectByIdAsync(Guid id);
        Task<bool> UpdateProjectAsync(ProjectDto dto);
    }
}
