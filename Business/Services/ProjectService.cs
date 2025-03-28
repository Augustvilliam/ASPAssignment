
using System.Runtime;
using Business.Interface;
using Data.Entities;
using Data.Interface;
using Domain.Models;
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

    public async Task CreateProjectAsync(ProjectCreateForm form)
    {
        await _projectRepo.BeginTransactionAsync();
        try
        {
            var project = new ProjectEntity
            {
                ProjectName = form.ProjectName,
                ClientName = form.ClientName,
                Description = form.Description,
                StartDate = form.StartDate,
                EndDate = form.EndDate,
                Budget = form.Budget
            };

            foreach (var memberId in form.SelectedMembersIds)
            {
                var member = await _memberRepo.GetByIdAsync(memberId);
                if (member != null)
                    project.Members.Add(member);
            }

            if (form.ProjectImage != null && form.ProjectImage.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}_{form.ProjectImage.FileName}";
                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await form.ProjectImage.CopyToAsync(stream);

                project.ProjectImagePath = $"/uploads/{fileName}";
            }
            await _projectRepo.CreateAsync(project);
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
