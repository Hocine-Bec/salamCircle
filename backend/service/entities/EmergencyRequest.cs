using service.enums;

namespace service.entities;

public class EmergencyRequest : BaseEntity
{
    public Guid CircleId { get; set; }
    public Guid RequestedById { get; set; }
    public decimal AmountRequested { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? SupportingContext { get; set; }
    public EmergencyStatus Status { get; set; } = EmergencyStatus.Pending;
    public Guid? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }

    // Navigation
    public Circle Circle { get; set; } = null!;
    public CircleMember RequestedBy { get; set; } = null!;
    public User? ReviewedBy { get; set; }
}
