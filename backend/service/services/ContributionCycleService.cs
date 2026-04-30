using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class ContributionCycleService : IContributionCycleService
{
    private readonly IContributionCycleRepository _cycleRepository;
    private readonly ICircleRepository _circleRepository;
    private readonly ICircleMemberRepository _circleMemberRepository;
    private readonly INotificationService _notificationService;
    private readonly ITransparencyLogService _transparencyLogService;

    public ContributionCycleService(
        IContributionCycleRepository cycleRepository,
        ICircleRepository circleRepository,
        ICircleMemberRepository circleMemberRepository,
        INotificationService notificationService,
        ITransparencyLogService transparencyLogService)
    {
        _cycleRepository = cycleRepository;
        _circleRepository = circleRepository;
        _circleMemberRepository = circleMemberRepository;
        _notificationService = notificationService;
        _transparencyLogService = transparencyLogService;
    }

    public async Task<ContributionCycle> GetActiveCycleAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.");

        // ⚠️ fix: use IsActiveMemberAsync
        if (!await _circleMemberRepository.IsActiveMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        var cycle = await _cycleRepository.GetActiveByCircleIdAsync(circleId)
            ?? throw new KeyNotFoundException("No active cycle found.");

        return cycle;
    }

    public async Task<List<ContributionCycle>> GetAllCyclesAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.");

        // ⚠️ fix: was missing member check entirely — any user could call this
        if (!await _circleMemberRepository.IsActiveMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        return await _cycleRepository.GetByCircleIdAsync(circleId);
    }

    public async Task<ContributionCycle> StartNextCycleAsync(Guid circleId, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var circle = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException("Circle not found.");

        if (circle.ImamId != imamId)
            throw new UnauthorizedAccessException("Only the imam may start the next cycle.");

        var month = DateTime.UtcNow.Month;
        var year = DateTime.UtcNow.Year;

        if (await _cycleRepository.GetByCircleAndMonthAsync(circleId, month, year) is not null)
            throw new InvalidOperationException("A cycle already exists for the current month and year.");

        var activeCycle = await _cycleRepository.GetActiveByCircleIdAsync(circleId);
        if (activeCycle is not null)
        {
            activeCycle.Status = CycleStatus.Completed;
            await _cycleRepository.UpdateAsync(activeCycle);
        }

        var allCycles = await _cycleRepository.GetByCircleIdAsync(circleId);

        var nextCycle = new ContributionCycle
        {
            CircleId = circleId,
            CycleNumber = allCycles.Count + 1,
            Month = month,
            Year = year,
            ContributorsPerCycle = circle.ContributorsPerMonth,
            Status = CycleStatus.Active
        };

        var created = await _cycleRepository.CreateAsync(nextCycle);

        // Reset pause flags for all active/paused members at start of new cycle (US-24)
        var allMembers = await _circleMemberRepository.GetByCircleIdAsync(circleId);
        foreach (var member in allMembers.Where(m => m.Status == MemberStatus.Active || m.Status == MemberStatus.Paused))
        {
            member.HasPausedThisCycle = false;
            member.Status = MemberStatus.Active;
            await _circleMemberRepository.UpdateAsync(member);
        }

        // Notify members whose turn it is this cycle
        var activeMembers = allMembers
            .Where(m => m.Status == MemberStatus.Active)
            .OrderBy(m => m.QueuePosition)
            .ToList();

        if (activeMembers.Count > 0)
        {
            var offset = ((created.CycleNumber - 1) * created.ContributorsPerCycle) % activeMembers.Count;
            var dueMembers = Enumerable.Range(0, created.ContributorsPerCycle)
                .Select(i => activeMembers[(offset + i) % activeMembers.Count])
                .ToList();

            foreach (var dueMember in dueMembers)
            {
                await _notificationService.SendAsync(
                    userId: dueMember.UserId,
                    circleId: circleId,
                    type: NotificationType.ContributionDue,
                    title: "It is your turn to contribute this month",
                    body: $"Assalamu alaikum, it is your turn to contribute to the circle \"{circle.Name}\" this month inshaAllah. Minimum contribution: {circle.MinimumContribution}. Jazak Allahu khayran.");
            }
        }

        return created;
    }
}