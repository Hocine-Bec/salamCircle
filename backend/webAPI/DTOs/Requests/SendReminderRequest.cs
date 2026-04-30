using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class SendReminderRequest
{
    [Required]
    public Guid CircleId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;
}
