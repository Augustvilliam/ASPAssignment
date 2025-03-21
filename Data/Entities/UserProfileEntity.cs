using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace Data.Entities;

[Index(nameof(Email), IsUnique = true)]
public class UserProfileEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    [Required]
    public string Address { get; set; } = null!;
    public string? JobTitle { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; } 
    public string? ProfileImage { get; set; }
}
