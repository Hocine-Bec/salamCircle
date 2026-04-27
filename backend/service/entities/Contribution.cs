using service.enums;

namespace service.entities;

public class Contribution : BaseEntity
{
    public Guid CycleId { get; set; }
    public Guid CircleId { get; set; }
    public Guid MemberId { get; set; }
    public decimal Amount { get; set; }
    public ContributionStatus Status { get; set; } = ContributionStatus.Pending;
    public DateTime? ContributedAt { get; set; }

    // Navigation
    public ContributionCycle Cycle { get; set; } = null!;
    public Circle Circle { get; set; } = null!;
    public CircleMember Member { get; set; } = null!;
}
