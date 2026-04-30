using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using webAPI.Extensions;
using System.Security.Claims;
using webAPI.Mapping;

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
        Guid circleId)
    {

        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var cycle = await _cycleService.GetActiveCycleAsync(circleId, requestingUserId);
            return Ok(cycle.ToDto());
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
        Guid circleId)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var cycles = await _cycleService.GetAllCyclesAsync(circleId, requestingUserId);
            return Ok(cycles.Select(c => c.ToDto()).ToList());
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
        Guid circleId)
    {
        var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var cycle = await _cycleService.StartNextCycleAsync(circleId, imamId);
            return CreatedAtAction(nameof(GetActiveCycle), new { circleId }, cycle.ToDto());
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

    
}