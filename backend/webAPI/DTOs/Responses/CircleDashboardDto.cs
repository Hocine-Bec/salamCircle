namespace webAPI.DTOs.Responses;

public class CircleDashboardDto
{
    public Guid CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int MemberCount { get; set; }
    public int ContributorsPerMonth { get; set; }
    public List<CircleMemberDto> CurrentContributors { get; set; } = [];
    public List<CircleMemberDto> NextContributors { get; set; } = [];
}