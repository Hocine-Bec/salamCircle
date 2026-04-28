using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/emergency-requests")]
public class EmergencyRequestsController : ControllerBase
{
    private readonly IEmergencyRequestService _emergencyRequestService;

    public EmergencyRequestsController(IEmergencyRequestService emergencyRequestService)
    {
        _emergencyRequestService = emergencyRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmergencyRequestDto>>> GetCircleRequests(
        Guid circleId,
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var requests = await _emergencyRequestService.GetCircleRequestsAsync(circleId, requestingUserId);
            return Ok(requests.Select(ToDto).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<EmergencyRequestDto>> SubmitRequest(
        Guid circleId,
        EmergencyRequestDto dto,
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var created = await _emergencyRequestService.SubmitRequestAsync(
                circleId,
                dto.AmountRequested,
                dto.Description,
                dto.SupportingContext,
                requestingUserId);

            return CreatedAtAction(nameof(GetCircleRequests), new { circleId, requestingUserId }, ToDto(created));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{requestId:guid}/approve")]
    public async Task<ActionResult<EmergencyRequestDto>> ApproveRequest(
        Guid circleId,
        Guid requestId,
        [FromQuery] Guid imamId)
    {
        try
        {
            var updated = await _emergencyRequestService.ApproveRequestAsync(requestId, imamId);
            return Ok(ToDto(updated));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{requestId:guid}/reject")]
    public async Task<ActionResult<EmergencyRequestDto>> RejectRequest(
        Guid circleId,
        Guid requestId,
        [FromQuery] string rejectionReason,
        [FromQuery] Guid imamId)
    {
        try
        {
            var updated = await _emergencyRequestService.RejectRequestAsync(requestId, rejectionReason, imamId);
            return Ok(ToDto(updated));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{requestId:guid}/disburse")]
    public async Task<IActionResult> DisburseEmergencyFunds(
        Guid circleId,
        Guid requestId,
        [FromQuery] Guid imamId)
    {
        try
        {
            await _emergencyRequestService.DisburseEmergencyFundsAsync(requestId, imamId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static EmergencyRequestDto ToDto(EmergencyRequest request) => new()
    {
        Id = request.Id,
        CircleId = request.CircleId,
        RequestedById = request.RequestedById,
        AmountRequested = request.AmountRequested,
        Description = request.Description,
        SupportingContext = request.SupportingContext,
        Status = request.Status.ToString(),
        ReviewedById = request.ReviewedById,
        ReviewedAt = request.ReviewedAt,
        RejectionReason = request.RejectionReason,
        CreatedAt = request.CreatedAt
    };
}
