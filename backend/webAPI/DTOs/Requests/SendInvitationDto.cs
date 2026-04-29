namespace webAPI.DTOs.Requests;

public class SendInvitationDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public Guid ImamId { get; set; }
}