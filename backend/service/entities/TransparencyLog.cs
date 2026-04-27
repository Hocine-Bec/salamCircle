using service.enums;

namespace service.entities;

public class TransparencyLog : BaseEntity
{
    public Guid CircleId { get; set; }
    public LogEventType EventType { get; set; }
    public Guid? ActorId { get; set; }
    public Guid? TargetId { get; set; }
    public Guid? ReferenceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Metadata { get; set; } // JSON string

    // Navigation
    public Circle Circle { get; set; } = null!;
    public User? Actor { get; set; }
    public User? Target { get; set; }
}
