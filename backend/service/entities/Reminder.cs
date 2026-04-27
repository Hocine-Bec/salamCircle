namespace service.entities;

public class Reminder : BaseEntity
{
    public Guid CircleId { get; set; }
    public Guid SentById { get; set; }
    public Guid SentToId { get; set; }
    public Guid CycleId { get; set; }

    // Navigation
    public Circle Circle { get; set; } = null!;
    public User SentBy { get; set; } = null!;
    public CircleMember SentTo { get; set; } = null!;
    public ContributionCycle Cycle { get; set; } = null!;
}
