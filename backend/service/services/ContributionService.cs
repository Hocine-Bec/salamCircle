using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class ContributionService : IContributionService
{
    private readonly IContributionRepository _contributionRepository;
    private readonly IContributionCycleRepository _cycleRepository;
    private readonly ICircleRepository _circleRepository;
    private readonly ICircleMemberRepository _circleMemberRepository;
    private readonly IEmergencyRequestRepository _emergencyRequestRepository;
    private readonly ITransparencyLogService _transparencyLogService;
    private readonly INotificationService _notificationService;

    public ContributionService(
        IContributionRepository repository,
        IContributionCycleRepository cycleRepository,
        ICircleRepository circleRepository,
        ICircleMemberRepository circleMemberRepository,
        IEmergencyRequestRepository emergencyRequestRepository,  // ← NEW
        ITransparencyLogService transparencyLogService,
        INotificationService notificationService)
    {
        _contributionRepository = repository;
        _cycleRepository = cycleRepository;
        _circleRepository = circleRepository;
        _circleMemberRepository = circleMemberRepository;
        _emergencyRequestRepository = emergencyRequestRepository; // ← NEW
        _transparencyLogService = transparencyLogService;
        _notificationService = notificationService;
    }

    public async Task<Contribution> SubmitContributionAsync(Guid cycleId, decimal amount, Guid requestingUserId)
    {
        if (cycleId == Guid.Empty)
            throw new ArgumentException("Cycle id cannot be empty.", nameof(cycleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var cycle = await _cycleRepository.GetByIdAsync(cycleId)
            ?? throw new KeyNotFoundException($"Contribution cycle with id '{cycleId}' not found.");

        var circle = await _circleRepository.GetByIdAsync(cycle.CircleId)
            ?? throw new KeyNotFoundException($"Circle with id '{cycle.CircleId}' not found.");

        var member = await _circleMemberRepository.GetByCircleAndUserAsync(circle.Id, requestingUserId)
            ?? throw new KeyNotFoundException($"Member '{requestingUserId}' not found in circle '{circle.Id}'.");

        if (member.Status != MemberStatus.Active)
            throw new InvalidOperationException("Member is not active.");

        var activeMembers = (await _circleMemberRepository.GetByCircleIdAsync(circle.Id))
            .Where(m => m.Status == MemberStatus.Active)
            .OrderBy(m => m.QueuePosition)
            .ToList();

        if (activeMembers.Count == 0)
            throw new InvalidOperationException("No active members found for the circle.");

        var offset = ((cycle.CycleNumber - 1) * cycle.ContributorsPerCycle) % activeMembers.Count;
        var slots = Enumerable.Range(0, cycle.ContributorsPerCycle)
            .Select(i => activeMembers[(offset + i) % activeMembers.Count])
            .ToList();

        if (!slots.Any(m => m.Id == member.Id))
            throw new InvalidOperationException("It is not your turn to contribute this cycle.");

        if (await _contributionRepository.GetByCycleAndMemberAsync(cycleId, member.Id) is not null)
            throw new InvalidOperationException("You have already contributed this cycle.");

        if (amount < circle.MinimumContribution)
            throw new ArgumentException($"Amount must be at least {circle.MinimumContribution}.", nameof(amount));

        var contribution = new Contribution
        {
            CycleId = cycleId,
            CircleId = circle.Id,
            MemberId = member.Id,
            Amount = amount,
            Status = ContributionStatus.Completed,
            ContributedAt = DateTime.UtcNow
        };

        var createdContribution = await _contributionRepository.CreateAsync(contribution);

        await _transparencyLogService.LogAsync(
            circle.Id,
            LogEventType.ContributionMade,
            $"Member '{requestingUserId}' contributed {amount} in cycle '{cycleId}'.",
            actorId: requestingUserId);

        // US-12: Advance the queue — move this member to the end after contributing
        member.QueuePosition = activeMembers.Max(m => m.QueuePosition) + 1;
        await _circleMemberRepository.UpdateAsync(member);
        await CompactQueueAsync(circle.Id);

        await _notificationService.SendAsync(
            userId: requestingUserId,
            circleId: circle.Id,
            type: NotificationType.ContributionDue,
            title: "Contribution recorded — Jazak Allahu khayran",
            body: $"Your contribution of {amount} to the circle \"{circle.Name}\" has been successfully recorded. Barakallahu feek for supporting your community.");

        return createdContribution;
    }

    // US-12: Re-sequence positions after queue advance so there are no gaps
    private async Task CompactQueueAsync(Guid circleId)
    {
        var members = (await _circleMemberRepository.GetByCircleIdAsync(circleId))
            .Where(m => m.Status == MemberStatus.Active)
            .OrderBy(m => m.QueuePosition)
            .ToList();

        for (var i = 0; i < members.Count; i++)
        {
            if (members[i].QueuePosition != i + 1)
            {
                members[i].QueuePosition = i + 1;
                await _circleMemberRepository.UpdateAsync(members[i]);
            }
        }
    }

    public async Task<List<Contribution>> GetMemberContributionHistoryAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var member = await _circleMemberRepository.GetByCircleAndUserAsync(circleId, requestingUserId)
            ?? throw new KeyNotFoundException($"Member '{requestingUserId}' not found in circle '{circleId}'.");

        return await _contributionRepository.GetByCircleAndMemberAsync(circleId, member.Id);
    }

    public async Task<List<Contribution>> GetCycleContributionsAsync(Guid cycleId, Guid requestingUserId)
    {
        if (cycleId == Guid.Empty)
            throw new ArgumentException("Cycle id cannot be empty.", nameof(cycleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var cycle = await _cycleRepository.GetByIdAsync(cycleId)
            ?? throw new KeyNotFoundException($"Contribution cycle with id '{cycleId}' not found.");

        // fix: IsActiveMemberAsync
        if (!await _circleMemberRepository.IsActiveMemberAsync(cycle.CircleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{cycle.CircleId}'.");

        return await _contributionRepository.GetByCycleIdAsync(cycleId);
    }

    public async Task<decimal> GetCircleBalanceAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        // fix: IsActiveMemberAsync
        if (!await _circleMemberRepository.IsActiveMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        // US-07 fix: balance = total contributions - total emergency disbursements
        var totalIn = await _contributionRepository.GetTotalByCircleIdAsync(circleId);
        var totalOut = await _emergencyRequestRepository.GetTotalDisbursedAsync(circleId);

        return totalIn - totalOut;
    }
}