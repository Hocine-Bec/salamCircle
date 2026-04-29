namespace webAPI.DTOs.Requests;

public class UpdateCircleDto
{
    public string Name { get; set; } = string.Empty;
    public decimal MinimumContribution { get; set; }
}