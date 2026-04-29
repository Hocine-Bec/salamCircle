// DTOs/Requests/UpdateUserDto.cs
namespace webAPI.DTOs.Requests;

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}