using service.entities;

namespace service.interfaces.repositories;

public interface ICircleMemberRepository
{
    Task<CircleMember?> GetByIdAsync(Guid id);
    Task<CircleMember?> GetByCircleAndUserAsync(Guid circleId, Guid userId);
    Task<List<CircleMember>> GetByCircleIdAsync(Guid circleId);
    Task<int> GetMaxQueuePositionAsync(Guid circleId);
    Task<int> CountActiveAsync(Guid circleId);
    Task<bool> IsMemberAsync(Guid circleId, Guid userId);
    Task<CircleMember> CreateAsync(CircleMember member);
    Task<CircleMember> UpdateAsync(CircleMember member);
    Task DeleteAsync(Guid id);
}
