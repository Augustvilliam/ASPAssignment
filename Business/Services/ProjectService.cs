
using Business.Dtos;
using Business.Factories;
using Business.Interface;
using Data.Entities;
using Data.Interface;
using Microsoft.EntityFrameworkCore;


namespace Business.Services;

public class ProjectService : IProjectService //crud för projekt, använding av transaktioner osv
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
        // Keep create logic unchanged
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
        catch
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
            var context = _projectRepo.Context;

            // Hämta projekt med medlemmar
            var existingProject = await context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (existingProject == null)
            {
                await _projectRepo.RollbackTransactionsAync();
                return false;
            }

            //ny lista med MemberEntity
            var updatedMembers = new List<MemberEntity>();
            foreach (var memberId in dto.MemberIds)
            {
                var member = await _memberRepo.GetByIdAsync(memberId);
                if (member != null)
                    updatedMembers.Add(member);
            }

            //Tillämpa ändringar via factory
            ProjectFactory.UpdateEntity(existingProject, dto, updatedMembers);

            //Försök spara
            await context.SaveChangesAsync();

            // Commit om allt gick bra
            await _projectRepo.CommitTransactionAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            //Rulla tillbaka och försök igen, denna förblir oförändrad eftersom det fanns en massa problem under utvecklingen
            await _projectRepo.RollbackTransactionsAync();

            // Enkel retry: hämta om, applicera om och commit
            var context = _projectRepo.Context;
            var existingProject = await context.Projects
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
            await _projectRepo.BeginTransactionAsync();
            try
            {
                await context.SaveChangesAsync();
                await _projectRepo.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _projectRepo.RollbackTransactionsAync();
                return false;
            }
        }
        catch
        {
            //andra fel → rulla tillbaka
            await _projectRepo.RollbackTransactionsAync();
            throw;
        }
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid id) //hämta projekt entitet
    {
        var project = await _projectRepo.Context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == id);
        return project is null ? null : ProjectFactory.FromEntity(project);
    }

    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        // Unchanged delete logic
        await _projectRepo.BeginTransactionAsync();
        try
        {
            var context = _projectRepo.Context;
            var project = await context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                await _projectRepo.RollbackTransactionsAync();
                return false;
            }
            context.Projects.Remove(project);
            var affected = await context.SaveChangesAsync();
            if (affected == 0)
            {
                await _projectRepo.RollbackTransactionsAync();
                return false;
            }
            await _projectRepo.CommitTransactionAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            await _projectRepo.RollbackTransactionsAync();
            return false;
        }
        catch
        {
            await _projectRepo.RollbackTransactionsAync();
            throw;
        }
    }
    //räkna antal projekt för pagination
    public async Task<int> CountAsync(string? status)
    {
        var query = _projectRepo.Context.Projects.AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);
        return await query.CountAsync();
    }
    //hämta paginerade projekt och sortera dem efter startdatum
    public async Task<IEnumerable<ProjectDto>> GetPagedAsync(string? status, int skip, int take)
    {
        var query = _projectRepo.Context.Projects
            .Include(p => p.Members)
            .AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);

        var list = await query
            .OrderByDescending(p => p.StartDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return list.Select(ProjectFactory.FromEntity);
    }
    public async Task<bool> AddMembersToProjectAsync(Guid projectId, List<string> memberIds) //Lägger till medlemmar i projekt och hämtar de som redan är kopplade. funkade inte först och började sendan funka av sig själv
    {
        // Starta transaction
        await _projectRepo.BeginTransactionAsync();
        try
        {
            var context = _projectRepo.Context;
            //Hämta projekt inkl. redan tillagda medlemmar
            var project = await context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                await _projectRepo.RollbackTransactionsAync();
                return false;
            }

            //Hämta varje MemberEntity och lägg bara till om den inte redan finns
            foreach (var mid in memberIds)
            {
                if (project.Members.Any(m => m.Id == mid))
                    continue;

                var member = await _memberRepo.GetByIdAsync(mid);
                if (member != null)
                    project.Members.Add(member);
            }

            //Spara ändringarna
            await context.SaveChangesAsync();

            //Commit om allt gick bra
            await _projectRepo.CommitTransactionAsync();
            return true;
        }
        catch
        {
            // Något gick fel → rulla tillbaka
            await _projectRepo.RollbackTransactionsAync();
            return false;
        }
    }
}
