using service.enums;

namespace service.entities;

public class CircleMember : BaseEntity
{
    public Guid CircleId { get; set; }
    public Guid UserId { get; set; }
    public int QueuePosition { get; set; }
    public int SwapCount { get; set; } = 0;
    public MemberStatus Status { get; set; } = MemberStatus.Active;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RemovedAt { get; set; }

    public bool HasPausedThisCycle { get; set; } = false;

    // Navigation
    public Circle Circle { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<Contribution> Contributions { get; set; } = [];
    public ICollection<EmergencyRequest> EmergencyRequests { get; set; } = [];
    public ICollection<SwapRequest> SwapRequestsAsRequester { get; set; } = [];
    public ICollection<SwapRequest> SwapRequestsAsAcceptor { get; set; } = [];
    public ICollection<Reminder> ReceivedReminders { get; set; } = [];
}
