using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/contributions")]
public class ContributionController : ControllerBase
{
    private readonly IContributionService _service;

    public ContributionController(IContributionService service)
    {
        _service = service;
    }

    [HttpGet("cycle/{cycleId:guid}")]
    public async Task<IActionResult> GetByCycle(
        Guid circleId,
        Guid cycleId,
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
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetMyHistory(
        Guid circleId,
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
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance(
        Guid circleId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var balance = await _service.GetCircleBalanceAsync(circleId, requestingUserId);
            return Ok(new { circleId, balance });
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
    public async Task<IActionResult> Submit(
        Guid circleId,
        [FromBody] SubmitContributionRequest dto)
    {
        try
        {
            var contribution = await _service.SubmitContributionAsync(
                dto.CycleId,
                dto.Amount,
                dto.RequestingUserId);

            return CreatedAtAction(
                nameof(GetByCycle),
                new { circleId, cycleId = contribution.CycleId, requestingUserId = dto.RequestingUserId },
                ToDto(contribution));
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

    private static ContributionDto ToDto(Contribution c) => new()
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