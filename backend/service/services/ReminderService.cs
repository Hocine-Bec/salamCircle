using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _reminderRepository;
    private readonly IContributionCycleRepository _cycleRepository;
    private readonly ICircleRepository _circleRepository;
    private readonly ICircleMemberRepository _circleMemberRepository;
    private readonly ITransparencyLogService _transparencyLogService;

    private readonly INotificationService _notificationService;

    public ReminderService(
        IReminderRepository reminderRepository,
        IContributionCycleRepository cycleRepository,
        ICircleRepository circleRepository,
        ICircleMemberRepository circleMemberRepository,
        ITransparencyLogService transparencyLogService,
        INotificationService notificationService)
    {
        _reminderRepository = reminderRepository;
        _cycleRepository = cycleRepository;
        _circleRepository = circleRepository;
        _circleMemberRepository = circleMemberRepository;
        _transparencyLogService = transparencyLogService;
        _notificationService = notificationService;
    }

    public async Task SendReminderAsync(Guid cycleId, Guid targetMemberId, Guid imamId)
    {
        if (cycleId == Guid.Empty)
            throw new ArgumentException("Cycle id cannot be empty.", nameof(cycleId));

        if (targetMemberId == Guid.Empty)
            throw new ArgumentException("Target member id cannot be empty.", nameof(targetMemberId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var cycle = await _cycleRepository.GetByIdAsync(cycleId)
            ?? throw new KeyNotFoundException($"Contribution cycle with id '{cycleId}' not found.");

        var circle = await _circleRepository.GetByIdAsync(cycle.CircleId)
            ?? throw new KeyNotFoundException($"Circle with id '{cycle.CircleId}' not found.");

        if (circle.ImamId != imamId)
            throw new UnauthorizedAccessException("Only the Imam may send reminders.");

        var existingCount = await _reminderRepository.CountByCycleAndMemberAsync(cycleId, targetMemberId);
        if (existingCount >= 3)
            throw new InvalidOperationException("Maximum reminders reached for this member in this cycle.");

        var reminder = new Reminder
        {
            CircleId = circle.Id,
            CycleId = cycleId,
            SentToId = targetMemberId,
            SentById = imamId
        };

        var created = await _reminderRepository.CreateAsync(reminder);

        
        await _notificationService.SendAsync(
    userId: targetMemberId, 
    circleId: circle.Id,
    type: NotificationType.ReminderReceived,
    title: "Assalamu alaikum, gentle payment reminder",
    body: $"Assalamu alaikum , it is your turn to contribute to the circle \"{circle.Name}\" this month inshaAllah. Jazak Allahu khayran for supporting your community." );

        await _transparencyLogService.LogAsync(
            circle.Id,
            LogEventType.ReminderSent,
            $"Imam '{imamId}' sent a reminder to member '{targetMemberId}' in cycle '{cycleId}'.",
            actorId: imamId,
            targetId: targetMemberId,
            referenceId: created.Id);
    }

    public async Task<List<Reminder>> GetMemberRemindersAsync(Guid memberId, Guid requestingUserId)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member id cannot be empty.", nameof(memberId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var member = await _circleMemberRepository.GetByIdAsync(memberId)
            ?? throw new KeyNotFoundException($"Member with id '{memberId}' not found.");

        if (member.UserId != requestingUserId)
            throw new UnauthorizedAccessException("Members may only view their own reminders.");

        return await _reminderRepository.GetByMemberAsync(memberId);
    }
}