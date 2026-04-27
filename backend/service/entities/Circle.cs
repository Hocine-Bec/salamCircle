using service.enums;

namespace service.entities;

public class Circle : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid ImamId { get; set; }
    public decimal MinimumContribution { get; set; }
    public int ContributorsPerMonth { get; set; } = 1;
    public CircleStatus Status { get; set; } = CircleStatus.Active;
    public DateTime? ClosedAt { get; set; }

    // Navigation
    public User Imam { get; set; } = null!;
    public ICollection<CircleMember> Members { get; set; } = [];
    public ICollection<CircleInvitation> Invitations { get; set; } = [];
    public ICollection<ContributionCycle> Cycles { get; set; } = [];
    public ICollection<EmergencyRequest> EmergencyRequests { get; set; } = [];
    public ICollection<TransparencyLog> TransparencyLogs { get; set; } = [];
    public ICollection<Notification>    Notifications { get; set; } = [];
}
