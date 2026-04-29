using service.enums;

namespace webAPI.DTOs.Responses;

public class CircleInvitationDto
{
    public Guid Id { get; set; }
    public Guid CircleId { get; set; }
    public Guid InvitedById { get; set; }
    public Guid InvitedUserId { get; set; }
    public InvitationStatus Status { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}