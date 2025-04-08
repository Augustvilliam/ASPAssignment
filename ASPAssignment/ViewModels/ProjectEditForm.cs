using System.ComponentModel.DataAnnotations;

namespace ASPAssignment.ViewModels;

public class ProjectEditForm
{
    [Required]
    public string Id { get; set; } = null!;

    [Display(Name = "Project Image")]
    public IFormFile? ProjectImage { get; set; }

    public string? ExistingImagePath { get; set; }

    [Required(ErrorMessage = "Project name is required.")]
    [Display(Name = "Project Name")]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "Client name is required.")]
    [Display(Name = "Client Name")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "At least one member must be selected.")]
    [Display(Name = "Project Member")]
    public List<string> SelectedMemberId { get; set; } = [];

    [Required(ErrorMessage = "Budget is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Budget must be greater than 0.")]
    [Display(Name = "Budget")]
    public decimal Budget { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [Display(Name = "Project Status")]
    public string Status { get; set; } = "Ongoing";
}
