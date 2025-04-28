
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class ApplicationRole : IdentityRole
{
    public bool IsAdmin { get; set; }
}
