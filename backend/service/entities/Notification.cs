using service.enums;

namespace service.entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? CircleId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Circle? Circle { get; set; }
}
