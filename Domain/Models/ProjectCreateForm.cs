
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace Domain.Models;

public class ProjectCreateForm
{
    [Display(Name = "Project Image")]
    public IFormFile? ProjectImage { get; set; }

    [Required]
    [Display(Name = "Project Name")]
    public string ProjectName { get; set; } = null!;

    [Required]
    [Display(Name = "Client Name")]
    public string ClientName { get; set; } = null!;
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Now;
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; } = DateTime.Now;

    [Required]
    [Display(Name = "Project Members")]
    public List<string> SelectedMembersIds { get; set; } = [];

    [Required]
    [Display(Name = "Budget")]
    public decimal Budget { get; set; }
}
