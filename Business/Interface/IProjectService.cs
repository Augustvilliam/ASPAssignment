using Data.Entities;
using Business.Dtos;

namespace Business.Interface
{
    public interface IProjectService
    {
        Task CreateProjectAsync(ProjectDto dto);
        Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync();
    }
}
