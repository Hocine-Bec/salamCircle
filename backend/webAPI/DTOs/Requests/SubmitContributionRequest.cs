using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class SubmitContributionRequest
{
    [Required]
    public Guid CycleId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    // TODO: remove when JWT is wired
    public Guid RequestingUserId { get; set; }
}
