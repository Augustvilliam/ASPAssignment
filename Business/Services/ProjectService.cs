using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        // No explicit transaction needed for single SaveChanges
        var context = _projectRepo.Context;

        var existingProject = await context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == dto.Id);
        if (existingProject == null)
            return false;

        // Prepare new members list
        var updatedMembers = new List<MemberEntity>();
        foreach (var memberId in dto.MemberIds)
        {
            var member = await _memberRepo.GetByIdAsync(memberId);
            if (member != null)
                updatedMembers.Add(member);
        }

        // Apply changes
        ProjectFactory.UpdateEntity(existingProject, dto, updatedMembers);

        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Concurrency conflict detected. Reload and retry once.
            await context.Entry(existingProject).ReloadAsync();

            // Reapply changes
            ProjectFactory.UpdateEntity(existingProject, dto, updatedMembers);
            await context.SaveChangesAsync();
            return true;
        }
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid id)
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

    public async Task<int> CountAsync(string? status)
    {
        var query = _projectRepo.Context.Projects.AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);
        return await query.CountAsync();
    }

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
}
