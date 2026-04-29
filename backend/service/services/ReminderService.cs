using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _reminderRepository;
    private readonly IContributionCycleRepository _cycleRepository;
    private readonly ITransparencyLogService _transparencyLogService;

    public ReminderService(
        IReminderRepository reminderRepository,
        IContributionCycleRepository cycleRepository,
        ITransparencyLogService transparencyLogService)
    {
        _reminderRepository = reminderRepository;
        _cycleRepository = cycleRepository;
        _transparencyLogService = transparencyLogService;
    }

    public async Task SendReminderAsync(Guid cycleId, Guid targetMemberId, Guid imamId)
    {
        if (cycleId == Guid.Empty)
            throw new ArgumentException("Cycle id cannot be empty.", nameof(cycleId));

        if (targetMemberId == Guid.Empty)
            throw new ArgumentException("Target member id cannot be empty.", nameof(targetMemberId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var existingCount = await _reminderRepository
            .CountByCycleAndMemberAsync(cycleId, targetMemberId);

        if (existingCount >= 3)
            throw new InvalidOperationException(
                "Maximum reminders reached for this member in this cycle.");

        var reminder = new service.entities.Reminder
        {
            CycleId = cycleId,
            SentToId = targetMemberId,
            SentById = imamId
        };

        var created = await _reminderRepository.CreateAsync(reminder);

        var cycle = await _cycleRepository.GetByIdAsync(cycleId)
            ?? throw new KeyNotFoundException($"Contribution cycle with id '{cycleId}' not found.");

        await _transparencyLogService.LogAsync(
            cycle.CircleId,
            LogEventType.ReminderSent,
            $"Imam '{imamId}' sent a reminder to member '{targetMemberId}' in cycle '{cycleId}'.",
            actorId: imamId,
            targetId: targetMemberId,
            referenceId: created.Id);
    }

    public async Task<List<service.entities.Reminder>> GetMemberRemindersAsync(
        Guid memberId,
        Guid requestingUserId)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member id cannot be empty.", nameof(memberId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (memberId != requestingUserId)
            throw new UnauthorizedAccessException("Members may only view their own reminders.");

        return await _reminderRepository.GetByMemberAsync(memberId);
    }
}