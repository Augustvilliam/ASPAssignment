using Data.Entities;
using Domain.Models;

namespace Business.Interface
{
    public interface IProjectService
    {
        Task CreateProjectAsync(ProjectCreateForm form);
        Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync();
    }
}