using service.entities;

namespace service.interfaces.services;

public interface ICircleService
{
    Task<Circle> CreateCircleAsync(string name, decimal minimumContribution, Guid imamId);
    Task<Circle> GetByIdAsync(Guid circleId, Guid requestingUserId);
    Task<List<Circle>> GetUserCirclesAsync(Guid userId);
    Task<Circle> UpdateCircleAsync(Circle circle, Guid imamId);
    Task CloseCircleAsync(Guid circleId, Guid imamId);
}
