using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;

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
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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
        [FromBody] SubmitEmergencyRequestDto dto)
    {
        try
        {
            var created = await _emergencyRequestService.SubmitRequestAsync(
                circleId,
                dto.AmountRequested,
                dto.Description,
                dto.SupportingContext,
                dto.RequestingUserId);

            return CreatedAtAction(
                nameof(GetCircleRequests),
                new { circleId, requestingUserId = dto.RequestingUserId },
                ToDto(created));
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
        // TODO: replace with: var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
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
        [FromBody] RejectEmergencyRequestDto dto)
    {
        try
        {
            var updated = await _emergencyRequestService.RejectRequestAsync(
                requestId,
                dto.RejectionReason,
                dto.ImamId);

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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
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
        // TODO: replace with: var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static EmergencyRequestDto ToDto(EmergencyRequest r) => new()
    {
        Id = r.Id,
        CircleId = r.CircleId,
        RequestedById = r.RequestedById,
        AmountRequested = r.AmountRequested,
        Description = r.Description,
        SupportingContext = r.SupportingContext,
        Status = r.Status.ToString(),
        ReviewedById = r.ReviewedById,
        ReviewedAt = r.ReviewedAt,
        RejectionReason = r.RejectionReason,
        CreatedAt = r.CreatedAt
    };
}