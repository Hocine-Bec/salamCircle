using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class RejectEmergencyRequest
{
    [Required]
    [MaxLength(1000)]
    public string RejectionReason { get; set; } = string.Empty;

    // TODO: remove when JWT is wired
    [Required]
    public Guid ImamId { get; set; }
}
