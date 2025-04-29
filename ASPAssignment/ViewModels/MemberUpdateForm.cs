using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASPAssignment.ViewModels;

public class MemberUpdateForm
{
    public string Id { get; set; } = null!;

    [Required(ErrorMessage = "First name is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Role is required.")]
    [Display(Name = "Job Title / Role")]
    public string RoleId { get; set; } = null!;

    public IEnumerable<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address. Use format name@example.com")]
    [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w{2,4}$", ErrorMessage = "Invalid email address. Use format name@example.com")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [Display(Name = "Street Address")]
    public string? StreetAddress { get; set; }

    [Display(Name = "City")]
    public string? City { get; set; }

    [Display(Name = "Postal Code")]
    public string? PostalCode { get; set; }

    [Display(Name = "Birth Date")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [Display(Name = "Profile Picture")]
    public IFormFile? ProfilePic { get; set; }

    public string? ExistingProfileImagePath { get; set; }
}