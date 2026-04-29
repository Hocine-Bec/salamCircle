namespace webAPI.DTOs;

public class SubmitContributionRequest
{
    public Guid CycleId { get; set; }
    public decimal Amount { get; set; }
    public Guid RequestingUserId { get; set; }
}
