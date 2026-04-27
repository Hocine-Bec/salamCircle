using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs;

namespace webAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users.Select(ToDto).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
            return NotFound(new { message = $"User with id '{id}' not found." });

        return Ok(ToDto(user));
    }

    [HttpGet("phone/{phone}")]
    public async Task<ActionResult<UserDto>> GetByPhone(string phone)
    {
        var user = await _userService.GetByPhoneAsync(phone);
        if (user is null)
            return NotFound(new { message = $"User with phone '{phone}' not found." });

        return Ok(ToDto(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(UserDto dto)
    {
        try
        {
            var user = new User
            {
                Name = dto.Name,
                Phone = dto.Phone
            };

            var created = await _userService.CreateAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, UserDto dto)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "URL id does not match body id." });

        try
        {
            var user = new User
            {
                Id = dto.Id,
                Name = dto.Name,
                Phone = dto.Phone
            };

            var updated = await _userService.UpdateAsync(user);
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
            await _userService.DeleteAsync(id);
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

    private static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Phone = user.Phone,
        CreatedAt = user.CreatedAt
    };
}
