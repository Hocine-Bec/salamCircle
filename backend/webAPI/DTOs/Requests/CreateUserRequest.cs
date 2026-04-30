using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class CreateUserRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Phone]
    public string Phone { get; set; } = string.Empty;
}
