using Microsoft.AspNetCore.Identity;

namespace ASPAssignment.Models;

public class ApplicationUser : IdentityUser
{
    public string? profileImage { get; set; }

}
