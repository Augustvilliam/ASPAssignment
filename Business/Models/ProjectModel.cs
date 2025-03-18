

using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class ProjectModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    [StringLength(100, ErrorMessage = "Name is too long")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Client name is required")]
    public string ClientName { get; set; }
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "At least one member is required")]
    public List<string> Members { get; set; } = new List<string>();

    [Required(ErrorMessage = "Budget is required")]
    [Range(1, double.MaxValue, ErrorMessage = "Budget must be greater than zero")]
    public decimal Budget { get; set; } 
}
