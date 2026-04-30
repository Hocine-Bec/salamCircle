using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.enums;
using service.interfaces.services;
using webAPI.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using webAPI.Extensions;
using System.Security.Claims;
using webAPI.Mapping;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/logs")]
[Authorize]
public class TransparencyLogsController : ControllerBase
{
    private readonly ITransparencyLogService _transparencyLogService;

    public TransparencyLogsController(ITransparencyLogService transparencyLogService)
    {
        _transparencyLogService = transparencyLogService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransparencyLogDto>>> GetCircleLog(
        Guid circleId)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var logs = await _transparencyLogService.GetCircleLogAsync(circleId, requestingUserId);
            return Ok(logs.Select(l => l.ToDto()).ToList());
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
        [FromQuery] LogEventType eventType)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var logs = await _transparencyLogService.GetCircleLogByTypeAsync(circleId, eventType, requestingUserId);
            return Ok(logs.Select(l => l.ToDto()).ToList());
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
