namespace webAPI.DTOs.Requests;

public class SendInvitationRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public Guid ImamId { get; set; }
}