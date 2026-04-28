using service.enums;

namespace webAPI.DTOs;

public class ContributionDto
{
    public Guid Id { get; set; }
    public Guid CycleId { get; set; }
    public Guid CircleId { get; set; }
    public Guid MemberId { get; set; }
    public decimal Amount { get; set; }
    public ContributionStatus Status { get; set; }
    public DateTime? ContributedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}