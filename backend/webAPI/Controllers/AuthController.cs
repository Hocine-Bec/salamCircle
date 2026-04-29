using Microsoft.AspNetCore.Mvc;
using service.interfaces.services;
using webAPI.DTOs.Requests;

namespace webAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var token = await _authService.RegisterAsync(new service.DTOs.RegisterRequest
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password
            });

            return Ok(new { token });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var token = await _authService.LoginAsync(new service.DTOs.LoginRequest
            {
                Email = request.Email,
                Password = request.Password
            });

            return Ok(new { token });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
