using System.ComponentModel.DataAnnotations;

namespace ASPAssignment.ViewModels;

public class CreateRole
{
    [Required]
    [Display(Name = "Rollnamn")]
    public string Name { get; set; } = null!;

    [Display(Name = "Admin-behörighet")]
    public bool IsAdmin { get; set; }  
}
