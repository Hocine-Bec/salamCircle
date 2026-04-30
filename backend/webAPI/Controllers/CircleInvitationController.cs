using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using System.Security.Claims;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;
using webAPI.Extensions;
using webAPI.Mapping;

namespace webAPI.Controllers;

[ApiController]
[Route("api/circles/{circleId:guid}/invitations")]
[Authorize]
public class CircleInvitationController : ControllerBase
{
    private readonly IInvitationService _service;

    public CircleInvitationController(IInvitationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Send(Guid circleId, [FromBody] SendInvitationRequest dto)
    {
        try
        {
            var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.SendInvitationAsync(circleId, dto.PhoneNumber, imamId);
            return Ok(result.ToDto());
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
    public async Task<IActionResult> GetCircleInvitations(Guid circleId)
    {
        try
        {
            var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.GetCircleInvitationsAsync(circleId, imamId);
            return Ok(result.Select(i => i.ToDto()));
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
    public async Task<IActionResult> Accept(Guid circleId, Guid invitationId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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
    public async Task<IActionResult> Decline(Guid circleId, Guid invitationId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(Guid circleId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.GetPendingInvitationsAsync(userId);
            return Ok(result.Select(i => i.ToDto()).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

  
}
