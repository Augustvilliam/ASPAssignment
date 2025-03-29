

namespace Business.Dtos;

public class MemberDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public string? ProfileImagePath { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}
