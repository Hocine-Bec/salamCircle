using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class UpdateCircleRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Minimum contribution must be greater than 0.")]
    public decimal MinimumContribution { get; set; }
}
