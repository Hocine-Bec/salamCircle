using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.enums;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/logs")]
public class TransparencyLogsController : ControllerBase
{
    private readonly ITransparencyLogService _transparencyLogService;

    public TransparencyLogsController(ITransparencyLogService transparencyLogService)
    {
        _transparencyLogService = transparencyLogService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransparencyLogDto>>> GetCircleLog(
        Guid circleId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var logs = await _transparencyLogService.GetCircleLogAsync(circleId, requestingUserId);
            return Ok(logs.Select(ToDto).ToList());
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

    [HttpGet("filter")]
    public async Task<ActionResult<List<TransparencyLogDto>>> GetCircleLogByType(
        Guid circleId,
        [FromQuery] LogEventType eventType,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var logs = await _transparencyLogService.GetCircleLogByTypeAsync(circleId, eventType, requestingUserId);
            return Ok(logs.Select(ToDto).ToList());
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

    private static TransparencyLogDto ToDto(TransparencyLog log) => new()
    {
        Id = log.Id,
        CircleId = log.CircleId,
        EventType = log.EventType.ToString(),
        ActorId = log.ActorId,
        TargetId = log.TargetId,
        ReferenceId = log.ReferenceId,
        Description = log.Description,
        Metadata = log.Metadata,
        CreatedAt = log.CreatedAt
    };
}
