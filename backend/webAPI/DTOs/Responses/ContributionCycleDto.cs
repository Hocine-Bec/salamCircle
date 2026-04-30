// ContributionCycleDto.cs
using service.enums;

namespace webAPI.DTOs.Responses;

public class ContributionCycleDto
{
    public Guid Id { get; set; }
    public Guid CircleId { get; set; }
    public int CycleNumber { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }

    public int ContributorsPerCycle { get; set; } // ← add this
    public CycleStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}