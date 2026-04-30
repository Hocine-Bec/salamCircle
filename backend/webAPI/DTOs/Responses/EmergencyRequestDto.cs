namespace webAPI.DTOs.Responses;

public class EmergencyRequestDto
{
    public Guid Id { get; set; }
    public Guid CircleId { get; set; }
    public Guid RequestedById { get; set; }
    public decimal AmountRequested { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? SupportingContext { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
}
