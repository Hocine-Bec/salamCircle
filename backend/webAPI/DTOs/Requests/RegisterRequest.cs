using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class RegisterRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]   // ← was 6, US-01 requires min 8
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}