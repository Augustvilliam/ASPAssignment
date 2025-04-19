

using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ChatMessageEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    required public string SenderId { get; set; } = null!;

    [Required]
    required public string RecipientId { get; set; } = null!;

    [Required]
    public string Text { get; set; } = null!;

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

}
