using System.ComponentModel.DataAnnotations;

namespace webAPI.DTOs.Requests;

public class CreateSwapRequest
{
    [Required]
    public Guid CycleId { get; set; }

}
