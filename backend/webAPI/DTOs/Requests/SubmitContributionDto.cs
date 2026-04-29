namespace webAPI.DTOs.Requests;

public class SubmitContributionDto
{
    public Guid CycleId { get; set; }
    public decimal Amount { get; set; }
    // TODO: remove when JWT is wired
    public Guid RequestingUserId { get; set; }
}