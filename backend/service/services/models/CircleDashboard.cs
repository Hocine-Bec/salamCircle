using service.entities;

namespace service.models;

public class CircleDashboard
{
    public Guid CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int MemberCount { get; set; }
    public int ContributorsPerMonth { get; set; }
    public List<CircleMember> CurrentContributors { get; set; } = [];
    public List<CircleMember> NextContributors { get; set; } = [];
}