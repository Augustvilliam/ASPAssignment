using Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

public class MemberEntity : IdentityUser
{
    public string? ProfileImagePath { get; set; }

    public virtual MemberProfileEntity? Profile { get; set; }

    public ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();

    // Convenience-egenskap för att få ut JobTitle från profilen
    [NotMapped]
    public string? JobTitle => Profile?.Role?.Name;
}