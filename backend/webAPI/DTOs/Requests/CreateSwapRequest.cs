// DTOs/Requests/PostSwapRequestDto.cs
namespace webAPI.DTOs.Requests;

public class CreateSwapRequest
{
    public Guid CycleId { get; set; }
    // TODO: remove when JWT is wired
    public Guid RequestingUserId { get; set; }
}