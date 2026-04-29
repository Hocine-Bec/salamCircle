using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContributionController : ControllerBase
{
    private readonly IContributionService _service;

    public ContributionController(IContributionService service)
    {
        _service = service;
    }

    [HttpGet("cycle/{cycleId}")]
    public async Task<IActionResult> GetByCycle(Guid cycleId, 
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var result = await _service.GetCycleContributionsAsync(cycleId, requestingUserId);
            return Ok(result.Select(ToDto));
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

    [HttpGet("member/{circleId}")]
    public async Task<IActionResult> GetByMember(Guid circleId, 
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var result = await _service.GetMemberContributionHistoryAsync(circleId, requestingUserId);
            return Ok(result.Select(ToDto));
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

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitContribution(SubmitContributionRequest request)
    {
        try
        {
            // TODO: replace RequestingUserId with JWT claim on that DTO field
            var contribution = await _service.SubmitContributionAsync(
                request.CycleId,
                request.Amount,
                request.RequestingUserId);

            return CreatedAtAction(nameof(GetByCycle), new { cycleId = contribution.CycleId, requestingUserId = request.RequestingUserId }, ToDto(contribution));
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

    private static ContributionDto ToDto(Contribution c)
    {
        return new ContributionDto
        {
            Id = c.Id,
            CycleId = c.CycleId,
            CircleId = c.CircleId,
            MemberId = c.MemberId,
            Amount = c.Amount,
            Status = c.Status,
            ContributedAt = c.ContributedAt,
            CreatedAt = c.CreatedAt
        };
    }
}