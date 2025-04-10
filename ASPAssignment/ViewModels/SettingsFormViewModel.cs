using System.ComponentModel.DataAnnotations;

namespace ASPAssignment.ViewModels;

public class SettingsFormViewModel
{
    public string Id { get; set; } = null!;

    [Display(Name = "Profile Picture")]
    public IFormFile? ProfileImage { get; set; }
    public string? ExistingProfileImagePath { get; set; }

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;

    [Phone]
    public string? Phone { get; set; }

    [Display(Name = "Street Address")]
    public string? StreetAddress { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }
}
