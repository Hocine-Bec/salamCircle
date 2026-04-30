using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.interfaces.services;
using System.Security.Claims;
using webAPI.DTOs.Responses;
using webAPI.Mapping;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/members")]
[Authorize]
public class CircleMembersController : ControllerBase
{
    private readonly ICircleMemberService _circleMemberService;

    public CircleMembersController(ICircleMemberService circleMemberService)
    {
        _circleMemberService = circleMemberService;
    }

    private Guid GetRequestingUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<CircleMemberDto>>> GetMembers(Guid circleId)
    {
        try
        {
            var members = await _circleMemberService.GetCircleMembersAsync(circleId, GetRequestingUserId());
            return Ok(members.Select(m => m.ToDto()).ToList());
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

    [HttpGet("queue")]
    public async Task<ActionResult<List<CircleMemberDto>>> GetQueue(Guid circleId)
    {
        try
        {
            var queue = await _circleMemberService.GetContributionQueueAsync(circleId, GetRequestingUserId());
            return Ok(queue.Select(m => m.ToDto()).ToList());
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

    // Self-pause: a member can only pause themselves
    [HttpPost("{memberId:guid}/pause")]
    public async Task<IActionResult> PauseMember(Guid circleId, Guid memberId)
    {
        var requestingUserId = GetRequestingUserId();

        // memberId in route must match the JWT user — pause is self-service only
        if (memberId != requestingUserId)
            return Forbid();

        try
        {
            await _circleMemberService.PauseMemberAsync(circleId, requestingUserId);
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

    // Self-exit: a member can only exit themselves
    [HttpPost("{memberId:guid}/exit")]
    public async Task<IActionResult> ExitCircle(Guid circleId, Guid memberId)
    {
        var requestingUserId = GetRequestingUserId();

        // memberId in route must match the JWT user — exit is self-service only
        if (memberId != requestingUserId)
            return Forbid();

        try
        {
            await _circleMemberService.ExitCircleAsync(circleId, requestingUserId);
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

    // Imam-only: remove a specific member
    [HttpDelete("{memberId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid circleId, Guid memberId)
    {
        try
        {
            var imamId = GetRequestingUserId();
            await _circleMemberService.RemoveMemberAsync(circleId, memberId, imamId);
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
    }

    // Imam-only: shuffle the queue
    [HttpPost("queue/shuffle")]
    public async Task<IActionResult> ShuffleQueue(Guid circleId)
    {
        try
        {
            var imamId = GetRequestingUserId();
            await _circleMemberService.ShuffleQueueAsync(circleId, imamId);
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
    }

    // Imam-only: compact queue positions (no gaps after removals)
    [HttpPost("queue/compact")]
    public async Task<IActionResult> CompactQueue(Guid circleId)
    {
        try
        {
            // Only the imam should be able to trigger this manually
            var imamId = GetRequestingUserId();
            await _circleMemberService.CompactQueuePositionsAsync(circleId);
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
    }
}
