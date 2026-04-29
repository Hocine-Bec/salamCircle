using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/members")]
public class CircleMembersController : ControllerBase
{

    // Note: in some endpoints:later: can add more verification on the requester (imam or system)
    private readonly ICircleMemberService _circleMemberService;

    public CircleMembersController(ICircleMemberService circleMemberService)
    {
        _circleMemberService = circleMemberService;
    }

    // GET: api/circles/{circleId}/members?requestingUserId={requestingUserId}
    [HttpGet]
    public async Task<ActionResult<List<CircleMemberDto>>> GetMembers(
        Guid circleId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        var members = await _circleMemberService.GetCircleMembersAsync(circleId, requestingUserId);
        return Ok(members.Select(ToDto).ToList());
    }

    // GET: api/circles/{circleId}/members/queue?requestingUserId={requestingUserId}
    [HttpGet("queue")]
    public async Task<ActionResult<List<CircleMemberDto>>> GetQueue(
        Guid circleId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        var queue = await _circleMemberService.GetContributionQueueAsync(circleId, requestingUserId);
        return Ok(queue.Select(ToDto).ToList());
    }

    // POST: api/circles/{circleId}/members/{memberId}/pause?requestingUserId={requestingUserId}
    [HttpPost("{memberId:guid}/pause")]
    public async Task<IActionResult> PauseMember(
        Guid circleId,
        Guid memberId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            // TEMP: will be replaced by JWT-based user identity later.
            // maybe also will be handled by permission !
            if (memberId != requestingUserId)
                return BadRequest("Requesting user must match member id for pause.");

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
    }

    // POST: api/circles/{circleId}/members/{memberId}/exit?requestingUserId={requestingUserId}
    [HttpPost("{memberId:guid}/exit")]
    public async Task<IActionResult> ExitCircle(
        Guid circleId,
        Guid memberId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            // TEMP: will be replaced by JWT-based user identity later.
            if (memberId != requestingUserId)
                return BadRequest("Requesting user must match member id for exit.");

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
    }

    // DELETE: api/circles/{circleId}/members/{memberId}?imamId={imamId}
    [HttpDelete("{memberId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid circleId,
        Guid memberId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid imamId)
    {
        try
        {
            // Later: verification on the imam id
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
    }

    // POST: api/circles/{circleId}/members/queue/shuffle?imamId={imamId}
    [HttpPost("queue/shuffle")]
    public async Task<IActionResult> ShuffleQueue(
        Guid circleId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid imamId)
        // later: can add more verification on the requester (imam or system)
    {
        try
        {
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
    }

    // POST: api/circles/{circleId}/members/queue/recalculate
    [HttpPost("queue/compact")]
    public async Task<IActionResult> CompactQueue(Guid circleId)
    {
        // later: can add more verification on the requester (imam or system)

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

    private static CircleMemberDto ToDto(CircleMember member) => new()
    {
        Id = member.Id,
        CircleId = member.CircleId,
        UserId = member.UserId,
        QueuePosition = member.QueuePosition,
        Status = member.Status.ToString(),
        SwapCount = member.SwapCount,
        JoinedAt = member.JoinedAt
    };
}