using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class MemberProfileEntity
{
    [Key]
    public string MemberId { get; set; } = null!;

    [ProtectedPersonalData]
    public string? FirstName { get; set; } = null!;

    [ProtectedPersonalData]
    public string? LastName { get; set; } = null!;

    [ProtectedPersonalData]
    public string? JobTitle { get; set; } = null!;

    [ProtectedPersonalData]
    public DateTime? BirthDate { get; set; }

    [ProtectedPersonalData]
    public string? StreetAddress { get; set; }

    [ProtectedPersonalData]
    public string? City { get; set; }

    [ProtectedPersonalData]
    public string? PostalCode { get; set; }

  
    [ForeignKey(nameof(MemberId))]
    public virtual MemberEntity Member { get; set; } = null!;
}
