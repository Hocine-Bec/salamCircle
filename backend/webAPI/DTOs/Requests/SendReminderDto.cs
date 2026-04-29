namespace webAPI.DTOs.Requests;

public class SendReminderDto
{
    public Guid CycleId { get; set; }
    public Guid TargetMemberId { get; set; }
    // TODO: remove when JWT is wired
    public Guid ImamId { get; set; }
}