using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CircleInvitationController : ControllerBase
{
    private readonly IInvitationService _service;

    public CircleInvitationController(IInvitationService service)
    {
        _service = service;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] CircleInvitationDto dto)
    {
        var result = await _service.SendInvitationAsync(
            dto.CircleId,
            "",
            dto.InvitedById
        );

        return Ok(ToDto(result));
    }

    [HttpGet("pending/{userId}")]
    public async Task<IActionResult> GetPending(Guid userId)
    {
        var result = await _service.GetPendingInvitationsAsync(userId);
        return Ok(result.Select(ToDto));
    }

    [HttpPost("accept/{id}")]
    public async Task<IActionResult> Accept(Guid id, [FromQuery] Guid userId)
    {
        await _service.AcceptInvitationAsync(id, userId);
        return Ok();
    }

    [HttpPost("decline/{id}")]
    public async Task<IActionResult> Decline(Guid id, [FromQuery] Guid userId)
    {
        await _service.DeclineInvitationAsync(id, userId);
        return Ok();
    }

    private static CircleInvitationDto ToDto(CircleInvitation i)
    {
        return new CircleInvitationDto
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
}