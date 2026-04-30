// DTOs/Requests/SubmitEmergencyRequestDto.cs
namespace webAPI.DTOs.Requests;

public class SubmitEmergencyRequest
{
    public decimal AmountRequested { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? SupportingContext { get; set; }
    // TODO: remove when JWT is wired
    public Guid RequestingUserId { get; set; }
}