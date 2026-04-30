using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class CircleMemberService : ICircleMemberService
{
    private readonly ICircleMemberRepository _circleMemberRepository;
    private readonly ICircleRepository _circleRepository;
    private readonly INotificationService _notificationService;
    private readonly ITransparencyLogService _transparencyLogService;

    public CircleMemberService(
        ICircleMemberRepository circleMemberRepository,
        ICircleRepository circleRepository,
        INotificationService notificationService,
        ITransparencyLogService transparencyLogService)
    {
        _circleMemberRepository = circleMemberRepository;
        _circleRepository = circleRepository;
        _notificationService = notificationService;
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

    public async Task<CircleMember> AddMemberAsync(Guid circleId, Guid userId, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var circle = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

        if (circle.ImamId != imamId)
            throw new UnauthorizedAccessException("Only the imam may add members.");

        if (await _circleMemberRepository.IsMemberAsync(circleId, userId))
            throw new InvalidOperationException("User is already an active member of the circle.");

        var maxPosition = await _circleMemberRepository.GetMaxQueuePositionAsync(circleId);
        var newMember = new CircleMember
        {
            CircleId = circleId,
            UserId = userId,
            QueuePosition = maxPosition + 1,
            Status = MemberStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        var createdMember = await _circleMemberRepository.CreateAsync(newMember);
        await UpdateContributorsPerMonthAsync(circleId);

        await _transparencyLogService.LogAsync(
            circleId,
            LogEventType.MemberJoined,
            $"Member '{userId}' joined the circle.",
            actorId: imamId,
            targetId: userId);

        return createdMember;
    }

    // Recalculates how many members contribute per month based on circle size,
    // then notifies everyone if the number goes up.
    private async Task UpdateContributorsPerMonthAsync(Guid circleId)
    {
        var circle = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

        var activeCount = (await _circleMemberRepository.GetByCircleIdAsync(circleId))
            .Count(m => m.Status == MemberStatus.Active);

        var newValue = Math.Max(1, activeCount / 10);
        if (newValue == circle.ContributorsPerMonth)
            return;

        var oldValue = circle.ContributorsPerMonth;
        circle.ContributorsPerMonth = newValue;
        await _circleRepository.UpdateAsync(circle);

        if (newValue > oldValue)
        {
            var activeMembers = (await _circleMemberRepository.GetByCircleIdAsync(circleId))
                .Where(m => m.Status == MemberStatus.Active)
                .ToList();

            foreach (var member in activeMembers)
            {
                await _notificationService.SendAsync(
                    member.UserId,
                    circleId,
                    NotificationType.CircleUpdated,
                    "Monthly contributors updated",
                    $"The number of monthly contributors in your circle has increased to {newValue}.");
            }
        }
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

        var circle = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

        // Soft delete — keep record for transparency log history (US-20)
        member.Status = MemberStatus.Removed;
        member.RemovedAt = DateTime.UtcNow;
        await _circleMemberRepository.UpdateAsync(member);

        await UpdateContributorsPerMonthAsync(circleId);
        await CompactQueuePositionsAsync(circleId);

        await _transparencyLogService.LogAsync(
            circleId,
            LogEventType.MemberRemoved,
            $"Member '{memberId}' was removed by Imam '{imamId}'.",
            actorId: imamId,
            targetId: memberId);

        // US-20: Removed member is notified and immediately loses access
        await _notificationService.SendAsync(
            userId: member.UserId,
            circleId: circleId,
            type: NotificationType.CircleUpdated,
            title: "You have been removed from a circle",
            body: $"You have been removed from the circle \"{circle.Name}\" by the Imam. Your contribution history remains accessible in the transparency log.");
    }

    public async Task PauseMemberAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var member = await _circleMemberRepository.GetByCircleAndUserAsync(circleId, requestingUserId)
            ?? throw new KeyNotFoundException($"Circle member for user '{requestingUserId}' in circle '{circleId}' not found.");

        if (member.HasPausedThisCycle)
            throw new InvalidOperationException("Member has already paused once this cycle.");

        member.Status = MemberStatus.Paused;
        member.HasPausedThisCycle = true;
        await _circleMemberRepository.UpdateAsync(member);

        // Member remains in queue; pause only skips this cycle's contribution.
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

        var circle = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

        member.Status = MemberStatus.Exited;
        await _circleMemberRepository.UpdateAsync(member);

        await UpdateContributorsPerMonthAsync(circleId);
        await CompactQueuePositionsAsync(circleId);

        await _transparencyLogService.LogAsync(
            circleId,
            LogEventType.MemberExited,
            $"Member '{requestingUserId}' exited the circle.",
            actorId: requestingUserId,
            targetId: requestingUserId);

        // US-25: Imam is notified immediately when a member exits
        await _notificationService.SendAsync(
            userId: circle.ImamId,
            circleId: circleId,
            type: NotificationType.CircleUpdated,
            title: "A member has exited your circle",
            body: $"A member has chosen to exit the circle \"{circle.Name}\". The queue has been updated accordingly.");
    }

    // Rotates the queue by moving the first member to the end
    public async Task ShuffleQueueAsync(Guid circleId, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var members = await _circleMemberRepository.GetByCircleIdAsync(circleId);
        if (members is null || members.Count == 0)
            throw new KeyNotFoundException($"No members found for circle '{circleId}'.");

        var imam = members.FirstOrDefault(m => m.UserId == imamId)
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

    // Returns the full ordered list of active members sorted by QueuePosition
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

    // Re-sequences QueuePosition values after a member leaves, so there are no gaps.
    public async Task CompactQueuePositionsAsync(Guid circleId)
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