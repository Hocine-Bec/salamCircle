using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class CreateCircleRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "MinimumContribution must be greater than zero.")]
    public decimal MinimumContribution { get; set; }
}
