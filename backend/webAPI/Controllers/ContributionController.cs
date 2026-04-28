using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContributionController : ControllerBase
{
    private readonly IContributionService _service;

    public ContributionController(IContributionService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var contribution = await _service.GetByIdAsync(id);

        if (contribution is null)
            return NotFound();

        return Ok(ToDto(contribution));
    }

    [HttpGet("cycle/{cycleId}")]
    public async Task<IActionResult> GetByCycle(Guid cycleId)
    {
        var result = await _service.GetByCycleIdAsync(cycleId);
        return Ok(result.Select(ToDto));
    }

    [HttpGet("member/{memberId}")]
    public async Task<IActionResult> GetByMember(Guid memberId)
    {
        var result = await _service.GetByMemberIdAsync(memberId);
        return Ok(result.Select(ToDto));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ContributionDto dto)
    {
        var contribution = new Contribution
        {
            CycleId = dto.CycleId,
            CircleId = dto.CircleId,
            MemberId = dto.MemberId,
            Amount = dto.Amount,
            Status = dto.Status,
            ContributedAt = dto.ContributedAt
        };

        var created = await _service.CreateAsync(contribution);

        return Ok(ToDto(created));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    private static ContributionDto ToDto(Contribution c)
    {
        return new ContributionDto
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
}