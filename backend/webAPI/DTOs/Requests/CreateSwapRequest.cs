using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class CreateSwapRequest
{
    [Required]
    public Guid CircleId { get; set; }

    [Required]
    public Guid RequestedWithUserId { get; set; }
}
