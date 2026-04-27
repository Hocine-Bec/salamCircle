using service.enums;

namespace service.entities;

public class CircleInvitation : BaseEntity
{
    public Guid CircleId { get; set; }
    public Guid InvitedById { get; set; }
    public Guid InvitedUserId { get; set; }
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime? RespondedAt { get; set; }

    // Navigation
    public Circle Circle { get; set; } = null!;
    public User InvitedBy { get; set; } = null!;
    public User InvitedUser { get; set; } = null!;
}
