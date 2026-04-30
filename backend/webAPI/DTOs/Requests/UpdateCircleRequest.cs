using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class UpdateCircleRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
}
