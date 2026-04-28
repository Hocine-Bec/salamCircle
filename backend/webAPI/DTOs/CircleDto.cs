using service.enums;

namespace webAPI.DTOs;

public class CircleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid ImamId { get; set; }

    public decimal MinimumContribution { get; set; }

    public int ContributorsPerMonth { get; set; }

    public CircleStatus Status { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}