using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/invitations")]
public class CircleInvitationController : ControllerBase
{
    private readonly IInvitationService _service;

    public CircleInvitationController(IInvitationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Send(Guid circleId, [FromBody] SendInvitationDto dto)
    {
        try
        {
            var result = await _service.SendInvitationAsync(
                circleId,
                dto.PhoneNumber,
                dto.ImamId);

            return Ok(ToDto(result));
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

    [HttpGet]
    public async Task<IActionResult> GetCircleInvitations(
        Guid circleId,
        // TODO: replace with: var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid imamId)
    {
        try
        {
            var result = await _service.GetCircleInvitationsAsync(circleId, imamId);
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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpPost("{invitationId:guid}/accept")]
    public async Task<IActionResult> Accept(
        Guid circleId,
        Guid invitationId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid userId)
    {
        try
        {
            await _service.AcceptInvitationAsync(invitationId, userId);
            return Ok();
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

    [HttpPost("{invitationId:guid}/decline")]
    public async Task<IActionResult> Decline(
        Guid circleId,
        Guid invitationId,
        // TODO: replace with: var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        [FromQuery] Guid userId)
    {
        try
        {
            await _service.DeclineInvitationAsync(invitationId, userId);
            return Ok();
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

    [HttpGet("pending/{userId:guid}")]
    public async Task<IActionResult> GetPending(Guid circleId, Guid userId)
    {
        try
        {
            var result = await _service.GetPendingInvitationsAsync(userId);
            return Ok(result.Select(ToDto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static CircleInvitationDto ToDto(CircleInvitation i) => new()
    {
        Id = i.Id,
        CircleId = i.CircleId,
        InvitedById = i.InvitedById,
        InvitedUserId = i.InvitedUserId,
        Status = i.Status,
        RespondedAt = i.RespondedAt,
        CreatedAt = i.CreatedAt
    };

}