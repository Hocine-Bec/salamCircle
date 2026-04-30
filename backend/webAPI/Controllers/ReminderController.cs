using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using webAPI.Extensions;


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
public async Task<IActionResult> SendReminder(Guid circleId, [FromBody] SendReminderRequest dto)
{
    try
    {
        var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _reminderService.SendReminderAsync(dto.CycleId, dto.TargetMemberId, imamId);
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
        Guid memberId)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var reminders = await _reminderService.GetMemberRemindersAsync(memberId, requestingUserId);
            return Ok(reminders.Select(r => r.ToDto()).ToList());
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


}