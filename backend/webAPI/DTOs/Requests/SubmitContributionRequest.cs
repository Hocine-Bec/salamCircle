namespace webAPI.DTOs.Requests;

public class SubmitContributionRequest
{
    public Guid CycleId { get; set; }
    public decimal Amount { get; set; }
    // TODO: remove when JWT is wired
    public Guid RequestingUserId { get; set; }
}