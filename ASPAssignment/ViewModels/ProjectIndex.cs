using Business.Dtos;
using Business.Helper;

namespace ASPAssignment.ViewModels;

public class ProjectIndex
{
    public IEnumerable<ProjectDto> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public string? Status { get; set; }
    public int AllCount { get; set; }
    public int OngoingCountAll { get; set; }
    public int CompletedCountAll { get; set; }
}
