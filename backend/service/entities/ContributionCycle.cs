using service.enums;

namespace service.entities;

public class ContributionCycle : BaseEntity
{
    public Guid CircleId { get; set; }
    public int CycleNumber { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public CycleStatus Status { get; set; } = CycleStatus.Active;

    // Navigation
    public Circle Circle { get; set; } = null!;
    public ICollection<Contribution> Contributions { get; set; } = [];
    public ICollection<SwapRequest> SwapRequests { get; set; } = [];
    public ICollection<Reminder> Reminders { get; set; } = [];
}
