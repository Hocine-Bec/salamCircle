// DTOs/Requests/RejectEmergencyRequestDto.cs
namespace webAPI.DTOs.Requests;

public class RejectEmergencyRequest
{
    public string RejectionReason { get; set; } = string.Empty;
    // TODO: remove when JWT is wired
    public Guid ImamId { get; set; }
}