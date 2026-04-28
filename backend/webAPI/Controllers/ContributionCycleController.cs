// ContributionCycleController.cs
using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContributionCycleController : ControllerBase
{
    private readonly IContributionCycleService _cycleService;

    public ContributionCycleController(IContributionCycleService cycleService)
    {
        _cycleService = cycleService;
    }

    [HttpGet("active/{circleId:guid}")]
    public async Task<ActionResult<ContributionCycleDto>> GetActiveCycle(
        Guid circleId,
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var cycle = await _cycleService.GetActiveCycleAsync(circleId, requestingUserId);
            return Ok(ToDto(cycle));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("circle/{circleId:guid}")]
    public async Task<ActionResult<List<ContributionCycleDto>>> GetAllCycles(
        Guid circleId,
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var cycles = await _cycleService.GetAllCyclesAsync(circleId, requestingUserId);
            return Ok(cycles.Select(ToDto).ToList());
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("start-next/{circleId:guid}")]
    public async Task<ActionResult<ContributionCycleDto>> StartNextCycle(
        Guid circleId,
        [FromQuery] Guid imamId)
    {
        try
        {
            var cycle = await _cycleService.StartNextCycleAsync(circleId, imamId);
            return Ok(ToDto(cycle));
        }
        catch (Exception ex)
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
        CreatedAt = cycle.CreatedAt
    };
}