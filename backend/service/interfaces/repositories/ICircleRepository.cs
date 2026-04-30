using service.entities;

namespace service.interfaces.repositories;

public interface ICircleRepository
{
    Task<Circle?> GetByIdAsync(Guid id);
    Task<List<Circle>> GetAllAsync();
    Task<List<Circle>> GetByImamIdAsync(Guid imamId);
    Task<List<Circle>> GetByUserIdAsync(Guid userId);
    Task<Circle> CreateAsync(Circle circle);
    Task<Circle> UpdateAsync(Circle circle);
    Task DeleteAsync(Guid id);
}
