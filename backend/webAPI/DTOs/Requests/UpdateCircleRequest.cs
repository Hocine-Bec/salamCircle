namespace webAPI.DTOs.Requests;

public class UpdateCircleRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal MinimumContribution { get; set; }
}