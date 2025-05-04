using System.ComponentModel.DataAnnotations;

namespace ASPAssignment.ViewModels;

public class CreateRole // själva skapaformuläret för creatrolls
{
    [Required]
    [Display(Name = "Rollnamn")]
    public string Name { get; set; } = null!;

    [Display(Name = "Admin-behörighet")]
    public bool IsAdmin { get; set; }  
}
