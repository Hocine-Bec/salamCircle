using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/cycles")]
[Authorize]
public class ContributionCycleController : ControllerBase
{
    private readonly IContributionCycleService _cycleService;

    public ContributionCycleController(IContributionCycleService cycleService)
    {
        _cycleService = cycleService;
    }

    [HttpGet("active")]
    public async Task<ActionResult<ContributionCycleDto>> GetActiveCycle(
        Guid circleId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var cycle = await _cycleService.GetActiveCycleAsync(circleId, requestingUserId);
            return Ok(ToDto(cycle));
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

    [HttpGet]
    public async Task<ActionResult<List<ContributionCycleDto>>> GetAllCycles(
        Guid circleId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var cycles = await _cycleService.GetAllCyclesAsync(circleId, requestingUserId);
            return Ok(cycles.Select(ToDto).ToList());
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

    [HttpPost("start-next")]
    public async Task<ActionResult<ContributionCycleDto>> StartNextCycle(
        Guid circleId,
        // TODO: replace with: var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid imamId)
    {
        try
        {
            var cycle = await _cycleService.StartNextCycleAsync(circleId, imamId);
            return CreatedAtAction(nameof(GetActiveCycle),
                new { circleId, requestingUserId = imamId },
                ToDto(cycle));
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

    private static ContributionCycleDto ToDto(ContributionCycle cycle) => new()
    {
        Id = cycle.Id,
        CircleId = cycle.CircleId,
        CycleNumber = cycle.CycleNumber,
        Month = cycle.Month,
        Year = cycle.Year,
        Status = cycle.Status,
        ContributorsPerCycle = cycle.ContributorsPerCycle,
        CreatedAt = cycle.CreatedAt
    };
}