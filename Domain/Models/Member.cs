

using Microsoft.AspNetCore.Http;

namespace Domain.Models;

public class Member
{
     
    public string Id { get; set; }
    public IFormFile? ProfilePic { get; set; }
    public string? ProfileImagePath { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? JobTitle { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone{ get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
