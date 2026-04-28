using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class CircleMemberService : ICircleMemberService
{
    private readonly ICircleMemberRepository _circleMemberRepository;
    private readonly ITransparencyLogService _transparencyLogService;

    public CircleMemberService(
        ICircleMemberRepository circleMemberRepository,
        ITransparencyLogService transparencyLogService)
    {
        _circleMemberRepository = circleMemberRepository;
        _transparencyLogService = transparencyLogService;
    }

    public async Task<List<CircleMember>> GetCircleMembersAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (!await _circleMemberRepository.IsMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        return await _circleMemberRepository.GetByCircleIdAsync(circleId);
    }

    public async Task RemoveMemberAsync(Guid circleId, Guid memberId, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (memberId == Guid.Empty)
            throw new ArgumentException("Member id cannot be empty.", nameof(memberId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var member = await _circleMemberRepository.GetByIdAsync(memberId)
            ?? throw new KeyNotFoundException($"Circle member with id '{memberId}' not found.");

        if (member.CircleId != circleId)
            throw new ArgumentException("Member does not belong to the specified circle.", nameof(circleId));

        // Soft delete — keep record for transparency log history (US-20)
        member.Status = MemberStatus.Removed;
        member.RemovedAt = DateTime.UtcNow;
        await _circleMemberRepository.UpdateAsync(member);

        // TODO: recalculate queue positions after removal (US-20)

        await _transparencyLogService.LogAsync(
            circleId,
            LogEventType.MemberRemoved,
            $"Member '{memberId}' was removed by Imam '{imamId}'.",
            actorId: imamId,
            targetId: memberId);
    }

    public async Task PauseMemberAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var member = await _circleMemberRepository.GetByCircleAndUserAsync(circleId, requestingUserId)
            ?? throw new KeyNotFoundException($"Circle member for user '{requestingUserId}' in circle '{circleId}' not found.");

        // 
        if (member.HasPausedThisCycle)
             throw new InvalidOperationException("Member has already paused once this cycle.");

        member.Status = MemberStatus.Paused;
        member.HasPausedThisCycle = true;
        await _circleMemberRepository.UpdateAsync(member);
        await _transparencyLogService.LogAsync(
            circleId,
            LogEventType.MemberPaused,
            $"Member '{requestingUserId}' paused their contribution turn.",
            actorId: requestingUserId,
            targetId: requestingUserId);
    }

    public async Task ExitCircleAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var member = await _circleMemberRepository.GetByCircleAndUserAsync(circleId, requestingUserId)
            ?? throw new KeyNotFoundException($"Circle member for user '{requestingUserId}' in circle '{circleId}' not found.");

        member.Status = MemberStatus.Exited;
        await _circleMemberRepository.UpdateAsync(member);
        await _transparencyLogService.LogAsync(
            circleId,
            LogEventType.MemberExited,
            $"Member '{requestingUserId}' exited the circle.",
            actorId: requestingUserId,
            targetId: requestingUserId);
    }

    public async Task ShuffleQueueAsync(Guid circleId, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var members = await _circleMemberRepository.GetByCircleIdAsync(circleId);
        if (members is null || members.Count == 0)
            throw new KeyNotFoundException($"No members found for circle '{circleId}'.");

        var imam = members.FirstOrDefault(m => m.Id == imamId)
            ?? throw new KeyNotFoundException($"Imam with id '{imamId}' is not a member of circle '{circleId}'.");

        var orderedMembers = members.OrderBy(m => m.QueuePosition).ToList();
        var firstMember = orderedMembers.First();
        orderedMembers.RemoveAt(0);
        orderedMembers.Add(firstMember);

        for (var index = 0; index < orderedMembers.Count; index++)
        {
            orderedMembers[index].QueuePosition = index + 1;
            await _circleMemberRepository.UpdateAsync(orderedMembers[index]);
        }

        await _transparencyLogService.LogAsync(
            circleId,
            LogEventType.QueueShuffled,
            $"Queue was shuffled by Imam '{imamId}'.",
            actorId: imamId);
    }

    public async Task<List<CircleMember>> GetContributionQueueAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (!await _circleMemberRepository.IsMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        var members = await _circleMemberRepository.GetByCircleIdAsync(circleId);
        return members
            .Where(m => m.Status == MemberStatus.Active)
            .OrderBy(m => m.QueuePosition)
            .ToList();
    }

    public async Task RecalculateContributorsPerMonthAsync(Guid circleId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        var members = await _circleMemberRepository.GetByCircleIdAsync(circleId);
        if (members is null || members.Count == 0)
            throw new KeyNotFoundException($"No members found for circle '{circleId}'.");

        var activeMembers = members
            .Where(m => m.Status == MemberStatus.Active)
            .OrderBy(m => m.QueuePosition)
            .ToList();

        for (var index = 0; index < activeMembers.Count; index++)
        {
            var member = activeMembers[index];
            var desiredPosition = index + 1;
            if (member.QueuePosition != desiredPosition)
            {
                member.QueuePosition = desiredPosition;
                await _circleMemberRepository.UpdateAsync(member);
            }
        }
    }
}
