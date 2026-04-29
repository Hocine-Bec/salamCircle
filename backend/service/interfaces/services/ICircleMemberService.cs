using service.entities;

namespace service.interfaces.services;

public interface ICircleMemberService
{
    Task<List<CircleMember>> GetCircleMembersAsync(Guid circleId, Guid requestingUserId);
    Task<CircleMember> AddMemberAsync(Guid circleId, Guid userId, Guid imamId);
    Task RemoveMemberAsync(Guid circleId, Guid memberId, Guid imamId);
    Task PauseMemberAsync(Guid circleId, Guid requestingUserId);
    Task ExitCircleAsync(Guid circleId, Guid requestingUserId);
    Task ShuffleQueueAsync(Guid circleId, Guid imamId);
    Task<List<CircleMember>> GetContributionQueueAsync(Guid circleId, Guid requestingUserId);
    Task CompactQueuePositionsAsync(Guid circleId);
}
