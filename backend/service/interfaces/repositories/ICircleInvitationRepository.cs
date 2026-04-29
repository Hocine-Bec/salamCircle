using service.entities;

namespace service.interfaces.repositories;

public interface ICircleInvitationRepository
{
    Task<CircleInvitation?> GetByIdAsync(Guid id);

    Task<List<CircleInvitation>> GetByCircleIdAsync(Guid circleId);
    Task<List<CircleInvitation>> GetPendingByUserIdAsync(Guid userId);
    Task<bool> ExistsAsync(Guid circleId, Guid userId);
    Task<CircleInvitation> CreateAsync(CircleInvitation invitation);
    Task<CircleInvitation> UpdateAsync(CircleInvitation invitation);
    Task DeleteAsync(Guid id);
}
