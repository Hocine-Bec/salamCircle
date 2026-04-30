using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/swap-requests")]
[Authorize]
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
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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
        [FromBody] CreateSwapRequest dto)
    {
        try
        {
            var created = await _swapService.PostSwapRequestAsync(dto.CycleId, dto.RequestingUserId);
            return CreatedAtAction(
                nameof(GetOpenSwapRequests),
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

    [HttpPost("{swapRequestId:guid}/accept")]
    public async Task<ActionResult<SwapRequestDto>> AcceptSwapRequest(
        Guid circleId,
        Guid swapRequestId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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

    private static SwapRequestDto ToDto(SwapRequest s) => new()
    {
        Id = s.Id,
        CircleId = s.CircleId,
        CycleId = s.CycleId,
        RequesterId = s.RequesterId,
        AcceptorId = s.AcceptorId,
        RequesterOriginalPosition = s.RequesterOriginalPosition,
        AcceptorOriginalPosition = s.AcceptorOriginalPosition,
        Status = s.Status.ToString(),
        AcceptedAt = s.AcceptedAt,
        CreatedAt = s.CreatedAt
    };
}