// DTOs/Requests/CreateUserDto.cs
namespace webAPI.DTOs.Requests;

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}