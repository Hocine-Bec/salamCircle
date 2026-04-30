using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using System.Security.Claims;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;
using webAPI.Extensions;


namespace webAPI.Controllers;

[ApiController]
[Route("api/circles")]
[Authorize]
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
        return Ok(circles.Select(c => c.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CircleDto>> GetById(Guid id)
    {
        var circle = await _circleService.GetByIdAsync(id);

        if (circle is null)
            return NotFound(new { message = $"Circle with id '{id}' not found." });

        return Ok(circle.ToDto());
    }

    [HttpGet("imam/{imamId:guid}")]
    public async Task<ActionResult<List<CircleDto>>> GetByImamId(Guid imamId)
    {
        try
        {
            var circles = await _circleService.GetByImamIdAsync(imamId);
            return Ok(circles.Select(c => c.ToDto()).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<CircleDto>> Create([FromBody] CreateCircleRequest dto)
    {
        try
        {
            var circle = new Circle
            {
                Name = dto.Name,
                ImamId = dto.ImamId,
                MinimumContribution = dto.MinimumContribution
            };

            var created = await _circleService.CreateAsync(circle);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
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

    [HttpPut("{id:guid}")]
public async Task<ActionResult<CircleDto>> Update(Guid id, [FromBody] UpdateCircleRequest dto)
{
    try
    {
        var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var circle = new Circle
        {
            Id = id,
            Name = dto.Name,
            MinimumContribution = dto.MinimumContribution
        };

        var updated = await _circleService.UpdateAsync(circle, imamId);
        return Ok(updated.ToDto());
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


    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id)
    {
        try
        {
            var imamId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _circleService.CloseCircleAsync(id, imamId);
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}