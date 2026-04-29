// DTOs/Responses/TransparencyLogDto.cs
namespace webAPI.DTOs.Responses;

public class TransparencyLogDto
{
    public Guid Id { get; set; }
    public Guid CircleId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Guid? ActorId { get; set; }
    public Guid? TargetId { get; set; }
    public Guid? ReferenceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
}