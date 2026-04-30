using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class SendReminderRequest
{
    [Required]
    public Guid CycleId { get; set; }

    [Required]
    public Guid TargetMemberId { get; set; }

    // TODO: remove when JWT is wired
    [Required]
    public Guid ImamId { get; set; }
}
