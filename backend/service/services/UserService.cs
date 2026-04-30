using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone number cannot be empty.", nameof(phone));

        return await _userRepository.GetByPhoneAsync(phone);
    }

    public async Task<List<User>> GetAllAsync()
        => await _userRepository.GetAllAsync();

    public async Task<User> CreateAsync(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(user.Name))
            throw new ArgumentException("Name cannot be empty.", nameof(user));

        if (string.IsNullOrWhiteSpace(user.Phone))
            throw new ArgumentException("Phone number cannot be empty.", nameof(user));

        if (await _userRepository.ExistsAsync(user.Phone))
            throw new InvalidOperationException($"Phone number '{user.Phone}' is already in use.");

        return await _userRepository.CreateAsync(user);
    }

    public async Task<User> UpdateAsync(User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        var existing = await _userRepository.GetByIdAsync(user.Id)
            ?? throw new KeyNotFoundException($"User with id '{user.Id}' not found.");

        // Ensure the new phone isn't already taken by another user
        if (existing.Phone != user.Phone)
        {
            var phoneOwner = await _userRepository.GetByPhoneAsync(user.Phone);
            if (phoneOwner is not null && phoneOwner.Id != user.Id)
                throw new InvalidOperationException($"Phone number '{user.Phone}' is already in use.");
        }

        existing.Name = user.Name;
        existing.Phone = user.Phone;

        return await _userRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        var existing = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User with id '{userId}' not found.");

        await _userRepository.DeleteAsync(userId);
    }
}
