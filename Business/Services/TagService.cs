using System.Linq;
using Business.Dtos;
using Business.Factories;
using Business.Interface;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Business.Services;

public class TagService : ITagService
{
    private readonly IMemberService _membersService;
    private readonly IProjectService _projectService;

    public TagService(IMemberService membersService, IProjectService projectService)
    {
        _membersService = membersService;
        _projectService = projectService;
    }

    public async Task<IEnumerable<MemberDto>> SearchMembersAsync(string term)
    {
        var all = await _membersService.GetAllMembersAsync();
        if (string.IsNullOrEmpty(term))
            return all;
        term = term.Trim().ToLowerInvariant();
        return all.Where(m =>
        m.FirstName?.ToLowerInvariant().Contains(term) == true ||
        m.LastName?.ToLowerInvariant().Contains(term) == true ||
        m.Email?.ToLowerInvariant().Contains(term) == true
        );
    }
    public async Task<IEnumerable<ProjectDto>> SearchProjectsAsync(string term)
    {
        var entities = await _projectService.GetAllProjectsAsync();
        var allDtos = entities.Select(ProjectFactory.FromEntity);

        if (string.IsNullOrWhiteSpace(term))
            return allDtos;

        term = term.Trim().ToLowerInvariant();
        return allDtos.Where(p =>
            (!string.IsNullOrEmpty(p.ProjectName) && p.ProjectName.ToLowerInvariant().Contains(term)) ||
            (!string.IsNullOrEmpty(p.ClientName) && p.ClientName.ToLowerInvariant().Contains(term)) ||
            (!string.IsNullOrEmpty(p.Description) && p.Description.ToLowerInvariant().Contains(term)) ||
            (!string.IsNullOrEmpty(p.Status) && p.Status.ToLowerInvariant().Contains(term))
        );
    }
}
