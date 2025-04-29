using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities;
using Microsoft.AspNetCore.Identity;

public class MemberProfileEntity
{
    [Key]
    public string MemberId { get; set; } = null!;

    [ProtectedPersonalData]
    public string? FirstName { get; set; }

    [ProtectedPersonalData]
    public string? LastName { get; set; }

    // JobTitle som fk till ApplicationRole
    [Required]
    [Display(Name = "Roll / Job Title")]
    public string RoleId { get; set; } = null!;

    [ForeignKey(nameof(RoleId))]
    public virtual ApplicationRole Role { get; set; } = null!;

    [ProtectedPersonalData]
    public DateTime? BirthDate { get; set; }

    [ProtectedPersonalData]
    public string? StreetAddress { get; set; }

    [ProtectedPersonalData]
    public string? City { get; set; }

    [ProtectedPersonalData]
    public string? PostalCode { get; set; }

    // Navigation till MemberEntity
    [ForeignKey(nameof(MemberId))]
    public virtual MemberEntity Member { get; set; } = null!;
}
