
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public bool IsComplete { get; set; }
    [Required]
    public DateTime StartDate { get; set; } =DateTime.UtcNow;
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
    public string? ImagePath { get; set; }
}
