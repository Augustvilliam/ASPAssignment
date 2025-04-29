
namespace Business.Dtos;

public class MemberDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string RoleId { get; set; } = null!;
    public string? JobTitle { get; set; }
    public string? ProfileImagePath { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public string? ProfileImageUrl { get; set; }
    public bool HasCompleteProfile { get; set; }
}