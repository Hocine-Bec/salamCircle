using service.entities;

namespace service.interfaces.repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByPhoneAsync(string phone);
    Task<bool> ExistsAsync(string phone);
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}
