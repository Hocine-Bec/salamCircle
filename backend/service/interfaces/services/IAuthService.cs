namespace service.interfaces.services;

public interface IAuthService
{
    Task<string> RegisterAsync(string name, string phone, string email, string password);
    Task<string> LoginAsync(string email, string password);
}