namespace webAPI.DTOs.Requests;

public class CreateCircleDto
{
    public string Name { get; set; } = string.Empty;
    public Guid ImamId { get; set; }
    public decimal MinimumContribution { get; set; }
}