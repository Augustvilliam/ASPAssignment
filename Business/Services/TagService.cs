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

    public async Task<IEnumerable<MemberDto>> SearchMembersAsync(string term) //söker bland members när de är laddade i dynamicview
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
    public async Task<IEnumerable<ProjectDto>> SearchProjectsAsync(string term) //söker bland projects när de är laddade i dynamicview
    {
        var entities = await _projectService.GetAllProjectsAsync();
        var allDtos = entities.Select(ProjectFactory.FromEntity).ToList();

        if (string.IsNullOrWhiteSpace(term))
            return allDtos;

        var t = term.Trim().ToLowerInvariant();
        return allDtos.Where(p =>
            // Sök på projektnamn, klient, beskrivning, status
            (!string.IsNullOrEmpty(p.ProjectName) && p.ProjectName.ToLowerInvariant().Contains(t)) ||
            (!string.IsNullOrEmpty(p.ClientName) && p.ClientName.ToLowerInvariant().Contains(t)) ||
            (!string.IsNullOrEmpty(p.Description) && p.Description.ToLowerInvariant().Contains(t)) ||
            (!string.IsNullOrEmpty(p.Status) && p.Status.ToLowerInvariant().Contains(t)) ||
            // Sök på medlemmar kopplade till projektet funkar sådär 
            (p.Members != null && p.Members.Any(m =>
                (!string.IsNullOrEmpty(m.FullName) && m.FullName.ToLowerInvariant().Contains(t)) ||
                (!string.IsNullOrEmpty(m.Email) && m.Email.ToLowerInvariant().Contains(t))
            ))
        );
    }
}
