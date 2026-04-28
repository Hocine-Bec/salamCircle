using service.entities;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class CircleService : ICircleService
{
    private readonly ICircleRepository _circleRepository;

    public CircleService(ICircleRepository circleRepository)
    {
        _circleRepository = circleRepository;
    }

    public async Task<Circle?> GetByIdAsync(Guid circleId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        return await _circleRepository.GetByIdAsync(circleId);
    }

    public async Task<List<Circle>> GetAllAsync()
        => await _circleRepository.GetAllAsync();

    public async Task<List<Circle>> GetByImamIdAsync(Guid imamId)
    {
        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        return await _circleRepository.GetByImamIdAsync(imamId);
    }

    public async Task<Circle> CreateAsync(Circle circle)
    {
        if (circle is null)
            throw new ArgumentNullException(nameof(circle));

        if (string.IsNullOrWhiteSpace(circle.Name))
            throw new ArgumentException("Circle name cannot be empty.", nameof(circle));

        if (circle.ImamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(circle));

        if (circle.MinimumContribution <= 0)
            throw new ArgumentException("Minimum contribution must be greater than zero.", nameof(circle));

        if (circle.ContributorsPerMonth <= 0)
            throw new ArgumentException("Contributors per month must be greater than zero.", nameof(circle));

        return await _circleRepository.CreateAsync(circle);
    }

    public async Task<Circle> UpdateAsync(Circle circle)
    {
        if (circle is null)
            throw new ArgumentNullException(nameof(circle));

        var existing = await _circleRepository.GetByIdAsync(circle.Id)
            ?? throw new KeyNotFoundException($"Circle with id '{circle.Id}' not found.");

        existing.Name = circle.Name;
        existing.ImamId = circle.ImamId;
        existing.MinimumContribution = circle.MinimumContribution;
        existing.ContributorsPerMonth = circle.ContributorsPerMonth;
        existing.Status = circle.Status;
        existing.ClosedAt = circle.ClosedAt;

        return await _circleRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(Guid circleId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        var existing = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

        await _circleRepository.DeleteAsync(circleId);
    }
}