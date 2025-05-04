using System.ComponentModel.DataAnnotations;

namespace ASPAssignment.ViewModels;

public class EditRole //för att editera en roll i adminview ändast för folk med IsAdmin=true.
{
    [Required]
    public string Id { get; set; }

    [Required]
    [Display(Name = "Rollnamn")]
    public string Name { get; set; }

    [Display(Name = "Admin-behörighet")]
    public bool IsAdmin { get; set; }
}
