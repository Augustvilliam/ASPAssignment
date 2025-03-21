using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASPAssignment.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    public bool IsAdmin { get; set; }

    public int? UserProfileId { get; set; }
}
