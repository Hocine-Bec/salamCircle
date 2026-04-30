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
    private readonly ITransparencyLogService _transparencyLogService;

    public ContributionService(
        IContributionRepository repository,
        IContributionCycleRepository cycleRepository,
        ICircleRepository circleRepository,
        ICircleMemberRepository circleMemberRepository,
        ITransparencyLogService transparencyLogService)
    {
        _contributionRepository = repository;
        _cycleRepository = cycleRepository;
        _circleRepository = circleRepository;
        _circleMemberRepository = circleMemberRepository;
        _transparencyLogService = transparencyLogService;
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


        // TODO: handle wrap-around when offset + ContributorsPerCycle exceeds member count
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

        return createdContribution;
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

        if (!await _circleMemberRepository.IsMemberAsync(cycle.CircleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{cycle.CircleId}'.");

        return await _contributionRepository.GetByCycleIdAsync(cycleId);
    }

    public async Task<decimal> GetCircleBalanceAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (!await _circleMemberRepository.IsMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        return await _contributionRepository.GetTotalByCircleIdAsync(circleId);
    }
}
