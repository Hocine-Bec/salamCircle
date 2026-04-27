using service.enums;

namespace service.entities;

public class SwapRequest : BaseEntity
{
    public Guid CircleId { get; set; }
    public Guid CycleId { get; set; }
    public Guid RequesterId { get; set; }
    public Guid? AcceptorId { get; set; }
    public int RequesterOriginalPosition { get; set; }
    public int? AcceptorOriginalPosition { get; set; }
    public SwapStatus Status { get; set; } = SwapStatus.Open;
    public DateTime? AcceptedAt { get; set; }

    // Navigation
    public Circle Circle { get; set; } = null!;
    public ContributionCycle Cycle { get; set; } = null!;
    public CircleMember Requester { get; set; } = null!;
    public CircleMember? Acceptor { get; set; }
}
