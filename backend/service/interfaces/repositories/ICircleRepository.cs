using service.entities;

namespace service.interfaces.repositories;

public interface ICircleRepository
{
    Task<Circle?> GetByIdAsync(Guid id);
    Task<List<Circle>> GetByUserIdAsync(Guid userId);
    Task<Circle> CreateAsync(Circle circle);
    Task<Circle> UpdateAsync(Circle circle);
}
