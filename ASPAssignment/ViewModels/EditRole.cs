using System.ComponentModel.DataAnnotations;

namespace ASPAssignment.ViewModels;

public class EditRole
{
    [Required]
    public string Id { get; set; }

    [Required]
    [Display(Name = "Rollnamn")]
    public string Name { get; set; }

    [Display(Name = "Admin-behörighet")]
    public bool IsAdmin { get; set; }
}
