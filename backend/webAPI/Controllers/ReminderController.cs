using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/reminders")]
[Authorize]
public class ReminderController : ControllerBase
{
    private readonly IReminderService _reminderService;

    public ReminderController(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    [HttpPost]
    public async Task<IActionResult> SendReminder(
        Guid circleId,
        [FromBody] SendReminderRequest dto)
    {
        try
        {
            await _reminderService.SendReminderAsync(dto.CycleId, dto.TargetMemberId, dto.ImamId);
            return Ok(new { message = "Reminder sent successfully." });
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

    [HttpGet("member/{memberId:guid}")]
    public async Task<ActionResult<List<ReminderDto>>> GetMemberReminders(
        Guid circleId,
        Guid memberId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var reminders = await _reminderService.GetMemberRemindersAsync(memberId, requestingUserId);
            return Ok(reminders.Select(ToDto).ToList());
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

    private static ReminderDto ToDto(Reminder r) => new()
    {
        Id = r.Id,
        CircleId = r.CircleId,
        CycleId = r.CycleId,
        SentById = r.SentById,
        SentToId = r.SentToId,
        CreatedAt = r.CreatedAt
    };
}