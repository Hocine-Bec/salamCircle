// ReminderController.cs
using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReminderController : ControllerBase
{
    private readonly IReminderService _reminderService;

    public ReminderController(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    [HttpPost("send")]
    public async Task<ActionResult> SendReminder(
        [FromQuery] Guid cycleId,
        [FromQuery] Guid targetMemberId,
        [FromQuery] Guid imamId)
    {
        try
        {
            await _reminderService.SendReminderAsync(cycleId, targetMemberId, imamId);
            return Ok(new { message = "Reminder sent successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("member/{memberId:guid}")]
    public async Task<ActionResult<List<ReminderDto>>> GetMemberReminders(
        Guid memberId,
        [FromQuery] Guid requestingUserId)
    {
        try
        {
            var reminders = await _reminderService
                .GetMemberRemindersAsync(memberId, requestingUserId);

            return Ok(reminders.Select(ToDto).ToList());
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static ReminderDto ToDto(Reminder reminder) => new()
    {
        Id = reminder.Id,
        CircleId = reminder.CircleId,
        SentById = reminder.SentById,
        SentToId = reminder.SentToId,
        CycleId = reminder.CycleId,
        CreatedAt = reminder.CreatedAt
    };
}