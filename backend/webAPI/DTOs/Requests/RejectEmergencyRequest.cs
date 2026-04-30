using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class RejectEmergencyRequest
{
    [Required]
    [MaxLength(1000)]
    public string RejectionReason { get; set; } = string.Empty;

}
