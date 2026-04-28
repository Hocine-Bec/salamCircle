using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CircleController : ControllerBase
{
    private readonly ICircleService _circleService;

    public CircleController(ICircleService circleService)
    {
        _circleService = circleService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CircleDto>>> GetAll()
    {
        var circles = await _circleService.GetAllAsync();
        return Ok(circles.Select(ToDto).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CircleDto>> GetById(Guid id)
    {
        var circle = await _circleService.GetByIdAsync(id);

        if (circle is null)
            return NotFound(new { message = $"Circle with id '{id}' not found." });

        return Ok(ToDto(circle));
    }

    [HttpGet("imam/{imamId:guid}")]
    public async Task<ActionResult<List<CircleDto>>> GetByImamId(Guid imamId)
    {
        var circles = await _circleService.GetByImamIdAsync(imamId);
        return Ok(circles.Select(ToDto).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<CircleDto>> Create(CircleDto dto)
    {
        try
        {
            var circle = new Circle
            {
                Name = dto.Name,
                ImamId = dto.ImamId,
                MinimumContribution = dto.MinimumContribution,
                ContributorsPerMonth = dto.ContributorsPerMonth,
                Status = dto.Status,
                ClosedAt = dto.ClosedAt
            };

            var created = await _circleService.CreateAsync(circle);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                ToDto(created)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CircleDto>> Update(Guid id, CircleDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "URL id does not match body id." });

        try
        {
            var circle = new Circle
            {
                Id = dto.Id,
                Name = dto.Name,
                ImamId = dto.ImamId,
                MinimumContribution = dto.MinimumContribution,
                ContributorsPerMonth = dto.ContributorsPerMonth,
                Status = dto.Status,
                ClosedAt = dto.ClosedAt
            };

            var updated = await _circleService.UpdateAsync(circle);
            return Ok(ToDto(updated));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _circleService.DeleteAsync(id);
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

    private static CircleDto ToDto(Circle circle) => new()
    {
        Id = circle.Id,
        Name = circle.Name,
        ImamId = circle.ImamId,
        MinimumContribution = circle.MinimumContribution,
        ContributorsPerMonth = circle.ContributorsPerMonth,
        Status = circle.Status,
        ClosedAt = circle.ClosedAt,
        CreatedAt = circle.CreatedAt
    };
}