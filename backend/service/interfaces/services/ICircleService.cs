using service.entities;

namespace service.interfaces.services;

public interface ICircleService
{
    Task<List<Circle>> GetAllAsync();
    Task<Circle?> GetByIdAsync(Guid circleId);
    Task<List<Circle>> GetByImamIdAsync(Guid imamId);
    Task<Circle> CreateAsync(Circle circle);
    Task<Circle> UpdateAsync(Circle circle, Guid imamId);

    Task DeleteAsync(Guid circleId);
    Task CloseCircleAsync(Guid circleId, Guid imamId);
}
