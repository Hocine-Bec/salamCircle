using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class CreateCircleRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid ImamId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Minimum contribution must be greater than 0.")]
    public decimal MinimumContribution { get; set; }
}
