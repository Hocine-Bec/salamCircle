namespace webAPI.DTOs.Responses;

public class ReminderDto
{
    public Guid Id { get; set; }
    public Guid CircleId { get; set; }
    public Guid CycleId { get; set; }
    public Guid SentById { get; set; }
    public Guid SentToId { get; set; }
    public DateTime CreatedAt { get; set; }
}