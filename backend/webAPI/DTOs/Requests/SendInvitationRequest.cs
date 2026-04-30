using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class SendInvitationRequest
{
    [Required]
    public Guid CircleId { get; set; }

    [Required]
    public Guid InvitedUserId { get; set; }
}
