using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class SendInvitationRequest
{
    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

}
