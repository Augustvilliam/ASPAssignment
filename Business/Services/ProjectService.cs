
namespace Business.Services;

public class ProjectService
{

    public async Task<Project> GetProjectByIdAsync(string id)
    {
        return await _projectRepository.GetProjectByIdAsync(id);
    }
    public async Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        return await _projectRepository.GetAllProjectsAsync();
    }
    public async Task<bool> CreateProjectAsync(Project project)
    {
        return await _projectRepository.CreateProjectAsync(project);
    }
    public async Task<bool> UpdateProjectAsync(Project project)
    {
        return await _projectRepository.UpdateProjectAsync(project);
    }
    public async Task<bool> DeleteProjectAsync(string id)
    {
        return await _projectRepository.DeleteProjectAsync(id);
    }
}
