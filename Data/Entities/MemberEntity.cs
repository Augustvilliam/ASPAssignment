using Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

public class MemberEntity : IdentityUser
{
    public string? ProfileImagePath { get; set; }

    public virtual MemberProfileEntity? Profile { get; set; }

    public ICollection<ProjectEntity> Projects { get; set; } = [];
}