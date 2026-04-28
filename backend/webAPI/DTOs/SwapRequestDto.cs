namespace webAPI.DTOs;

public class SwapRequestDto
{
    public Guid Id { get; set; }
    public Guid CircleId { get; set; }
    public Guid CycleId { get; set; }
    public Guid RequesterId { get; set; }
    public Guid? AcceptorId { get; set; }
    public int RequesterOriginalPosition { get; set; }
    public int? AcceptorOriginalPosition { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? AcceptedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
