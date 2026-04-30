namespace webAPI.DTOs.Requests;

public class CreateCircleRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid ImamId { get; set; }
    public decimal MinimumContribution { get; set; }
}