using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Responses;

namespace webAPI.Controllers;

[ApiController]
[Route("api/users/{userId:guid}/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetUserNotifications(Guid userId)
    {
        try
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications.Select(ToDto).ToList());
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

    [HttpGet("unread-count")]
    public async Task<ActionResult> GetUnreadCount(Guid userId)
    {
        try
        {
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
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

    [HttpPost("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid userId, Guid notificationId)
    {
        try
        {
            await _notificationService.MarkAsReadAsync(notificationId, userId);
            return NoContent();
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

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead(Guid userId)
    {
        try
        {
            await _notificationService.MarkAllAsReadAsync(userId);
            return NoContent();
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

    [HttpDelete("{notificationId:guid}")]
    public async Task<IActionResult> DeleteNotification(Guid userId, Guid notificationId)
    {
        try
        {
            await _notificationService.DeleteNotificationAsync(notificationId, userId);
            return NoContent();
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

    private static NotificationDto ToDto(Notification notification) => new()
    {
        Id = notification.Id,
        UserId = notification.UserId,
        CircleId = notification.CircleId,
        Type = notification.Type.ToString(),
        Title = notification.Title,
        Body = notification.Body,
        IsRead = notification.IsRead,
        ReadAt = notification.ReadAt,
        CreatedAt = notification.CreatedAt
    };
}
