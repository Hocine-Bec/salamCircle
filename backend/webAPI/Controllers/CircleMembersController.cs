using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using System.Security.Claims;
using webAPI.DTOs.Responses;
using webAPI.Extensions;


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

    [HttpGet]
    public async Task<ActionResult<List<CircleMemberDto>>> GetMembers(Guid circleId)
    {
        try
        {
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var members = await _circleMemberService.GetCircleMembersAsync(circleId, requestingUserId);
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
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var queue = await _circleMemberService.GetContributionQueueAsync(circleId, requestingUserId);
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

    [HttpPost("{memberId:guid}/pause")]
    public async Task<IActionResult> PauseMember(Guid circleId, Guid memberId)
    {
        try
        {
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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

    [HttpPost("{memberId:guid}/exit")]
    public async Task<IActionResult> ExitCircle(Guid circleId, Guid memberId)
    {
        try
        {
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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

    [HttpDelete("{memberId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid circleId, Guid memberId)
    {
        try
        {
            var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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

    [HttpPost("queue/shuffle")]
    public async Task<IActionResult> ShuffleQueue(Guid circleId)
    {
        try
        {
            var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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

    [HttpPost("queue/compact")]
    public async Task<IActionResult> CompactQueue(Guid circleId)
    {
        try
        {
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
