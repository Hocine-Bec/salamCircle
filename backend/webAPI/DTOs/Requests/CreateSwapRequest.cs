using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class CreateSwapRequest
{
    [Required]
    public Guid CycleId { get; set; }

    // TODO: remove when JWT is wired
    [Required]
    public Guid RequestingUserId { get; set; }
}
