using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/swap-requests")]
public class SwapRequestsController : ControllerBase
{
    private readonly ISwapService _swapService;

    public SwapRequestsController(ISwapService swapService)
    {
        _swapService = swapService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SwapRequestDto>>> GetOpenSwapRequests(
        Guid circleId,
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var requests = await _swapService.GetOpenSwapRequestsAsync(circleId, requestingUserId);
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
    public async Task<ActionResult<SwapRequestDto>> PostSwapRequest(
        Guid circleId,
        [FromQuery] Guid cycleId,
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var created = await _swapService.PostSwapRequestAsync(cycleId, requestingUserId);
            return CreatedAtAction(nameof(GetOpenSwapRequests), new { circleId, requestingUserId }, ToDto(created));
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

    [HttpGet("warning")]
    public async Task<ActionResult> CheckSwapWarning(
        Guid circleId,
        [FromQuery] Guid memberId)
    {
        try
        {
            var hasWarning = await _swapService.CheckSwapWarningAsync(circleId, memberId);
            return Ok(new { hasWarning });
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

    [HttpPost("{swapRequestId:guid}/accept")]
    public async Task<ActionResult<SwapRequestDto>> AcceptSwapRequest(
        Guid circleId,
        Guid swapRequestId,
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var updated = await _swapService.AcceptSwapRequestAsync(swapRequestId, requestingUserId);
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

    private static SwapRequestDto ToDto(SwapRequest swapRequest) => new()
    {
        Id = swapRequest.Id,
        CircleId = swapRequest.CircleId,
        CycleId = swapRequest.CycleId,
        RequesterId = swapRequest.RequesterId,
        AcceptorId = swapRequest.AcceptorId,
        RequesterOriginalPosition = swapRequest.RequesterOriginalPosition,
        AcceptorOriginalPosition = swapRequest.AcceptorOriginalPosition,
        Status = swapRequest.Status.ToString(),
        AcceptedAt = swapRequest.AcceptedAt,
        CreatedAt = swapRequest.CreatedAt
    };
}
