using Microsoft.AspNetCore.Mvc;
using service.entities;
using service.interfaces.services;
using webAPI.DTOs.Requests;
using webAPI.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using webAPI.Extensions;
using webAPI.Mapping;

namespace webAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
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
        return Ok(users.Select(u => u.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
            return NotFound(new { message = $"User with id '{id}' not found." });

        return Ok(user.ToDto());
    }

    [HttpGet("phone/{phone}")]
    public async Task<ActionResult<UserDto>> GetByPhone(string phone)
    {
        var user = await _userService.GetByPhoneAsync(phone);
        if (user is null)
            return NotFound(new { message = $"User with phone '{phone}' not found." });

        return Ok(user.ToDto());
    }

    

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserRequest dto)
    {
        try
        {
            var user = new User
            {
                Id = id,
                Name = dto.Name,
                Phone = dto.Phone
            };

            var updated = await _userService.UpdateAsync(user);
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

 
}