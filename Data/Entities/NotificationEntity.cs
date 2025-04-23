

using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class NotificationEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string UserId { get; set; } = null!;
    [Required]
    public string Message { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateTime Timestamp { get; set; }
    [Required]
    public string? NotificationType { get; set; }
}
