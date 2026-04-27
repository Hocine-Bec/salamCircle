using service.entities;

namespace service.interfaces.services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid userId);
    Task<User?> GetByPhoneAsync(string phone);
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid userId);
}
