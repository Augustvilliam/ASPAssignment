

namespace Business.Dtos;

public class NotificationDto
{
    public string ImageUrl { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public string NotificationType { get; set; }
    public string NotificationId { get; set; }
}
