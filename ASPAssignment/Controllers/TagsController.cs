using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;
    public TagsController(ITagService tagService) => _tagService = tagService;

    [HttpGet("members")]
    public async Task<IActionResult> Members([FromQuery] string? term = null)
    {
        var list = await _tagService.SearchMembersAsync(term);
        return Ok(list);
    }

    [HttpGet("projects")]
    public async Task<IEnumerable<ProjectDto>> Projects(string term)
        => await _tagService.SearchProjectsAsync(term);
}

