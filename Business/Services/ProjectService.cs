using Business.Dtos;
using Business.Factories;
using Business.Interface;
using Data.Entities;
using Data.Interface;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class ProjectService : IProjectService
{
    private readonly IGenericRepository<ProjectEntity> _projectRepo;
    private readonly IGenericRepository<MemberEntity> _memberRepo;

    public ProjectService(
        IGenericRepository<ProjectEntity> projectRepo,
        IGenericRepository<MemberEntity> memberRepo)
    {
        _projectRepo = projectRepo;
        _memberRepo = memberRepo;
    }

    public async Task CreateProjectAsync(ProjectDto dto)
    {
        await _projectRepo.BeginTransactionAsync();
        try
        {
            var memberEntities = new List<MemberEntity>();

            foreach (var memberId in dto.MemberIds)
            {
                var member = await _memberRepo.GetByIdAsync(memberId);
                if (member != null)
                    memberEntities.Add(member);
            }

            var entity = ProjectFactory.CreateEntity(dto, memberEntities);

            await _projectRepo.CreateAsync(entity);
            await _projectRepo.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _projectRepo.RollbackTransactionsAync();
            throw;
        }
    }
    public async Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync()
    {
        return await _projectRepo.Context.Projects
            .Include(p => p.Members)
            .ToListAsync();
    }
    public async Task<bool> UpdateProjectAsync(ProjectDto dto)
    {
        await _projectRepo.BeginTransactionAsync();

        try
        {
            var existingProject = await _projectRepo.Context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (existingProject == null)
                return false;

            var updatedMembers = new List<MemberEntity>();
            foreach (var memberId in dto.MemberIds)
            {
                var member = await _memberRepo.GetByIdAsync(memberId);
                if (member != null)
                    updatedMembers.Add(member);
            }

            ProjectFactory.UpdateEntity(existingProject, dto, updatedMembers);

            await _projectRepo.UpdateAsync(existingProject);
            await _projectRepo.CommitTransactionAsync();

            return true;
        }
        catch (Exception)
        {
            await _projectRepo.RollbackTransactionsAync();
            throw;
        }
    }
    public async Task<ProjectDto?> GetProjectByIdAsync(Guid id)
    {
        var project = await _projectRepo.Context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return null;

        return ProjectFactory.FromEntity(project);
    }
}
