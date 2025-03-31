
using System.ComponentModel.DataAnnotations;


namespace ASPAssignment.ViewModels;

public class MemberUpdateForm
{
    public string Id { get; set; } = null!;

    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = null!;

    public string? JobTitle { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address. Use format name@example.com")]
    [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w{2,4}$", ErrorMessage = "Invalid email address. Use format name@example.com")]
    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public IFormFile? ProfilePic { get; set; }

    public string? ExistingProfileImagePath { get; set; }

}
