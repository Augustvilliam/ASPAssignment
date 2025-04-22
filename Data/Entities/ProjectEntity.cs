
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ProjectName { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; } = DateTime.Now;

    [Column(TypeName = "money")]
    public decimal Budget { get; set; }
    public string? ProjectImagePath { get; set; }
    public string Status { get; set; } = "Ongoing";
    public ICollection<MemberEntity> Members { get; set; } = [];
    [Timestamp]
    [Column(TypeName = "rowversion")]
    public byte[] RowVersion { get; set; } = null!;
}
