using service.entities;

namespace service.interfaces.services;

public interface IAuthService
{
    Task<(User user, string token)> RegisterAsync(string name, string phone, string password);
    Task<(User user, string token)> LoginAsync(string phone, string password);
}
