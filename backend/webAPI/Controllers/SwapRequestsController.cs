using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using System.Security.Claims;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;
using webAPI.Extensions;


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
    public async Task<ActionResult<List<SwapRequestDto>>> GetOpenSwapRequests(Guid circleId)
    {
        try
        {
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var requests = await _swapService.GetOpenSwapRequestsAsync(circleId, requestingUserId);
            return Ok(requests.Select(s => s.ToDto()).ToList());
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
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var created = await _swapService.PostSwapRequestAsync(dto.CycleId, requestingUserId);
            return CreatedAtAction(
                nameof(GetOpenSwapRequests),
                new { circleId },
                created.ToDto());
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
        Guid swapRequestId)
    {
        try
        {
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updated = await _swapService.AcceptSwapRequestAsync(swapRequestId, requestingUserId);
            return Ok(updated.ToDto());
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


}
