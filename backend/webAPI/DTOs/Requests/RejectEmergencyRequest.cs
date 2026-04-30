using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class RejectEmergencyRequest
{
    [Required]
    public Guid EmergencyRequestId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;

    // TODO: remove when JWT is wired
    public Guid ImamId { get; set; }
}
