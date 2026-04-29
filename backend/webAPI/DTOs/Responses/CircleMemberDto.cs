namespace webAPI.DTOs.Responses;

public class CircleMemberDto
{
    public Guid Id { get; set; }
    public Guid CircleId { get; set; }
    public Guid UserId { get; set; }
    public int QueuePosition { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SwapCount { get; set; }
    public DateTime JoinedAt { get; set; }
}