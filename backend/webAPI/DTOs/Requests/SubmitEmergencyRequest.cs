using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class SubmitEmergencyRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal AmountRequested { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? SupportingContext { get; set; }


}
