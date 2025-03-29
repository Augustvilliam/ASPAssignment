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

}
