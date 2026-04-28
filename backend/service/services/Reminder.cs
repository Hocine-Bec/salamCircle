using service.entities;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _reminderRepository;

    public ReminderService(IReminderRepository reminderRepository)
    {
        _reminderRepository = reminderRepository;
    }

    public async Task SendReminderAsync(Guid cycleId, Guid targetMemberId, Guid imamId)
    {
        if (cycleId == Guid.Empty)
            throw new ArgumentException("Cycle id cannot be empty.");

        if (targetMemberId == Guid.Empty)
            throw new ArgumentException("Target member id cannot be empty.");

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.");

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

        await _reminderRepository.CreateAsync(reminder);
    }

    public async Task<List<service.entities.Reminder>> GetMemberRemindersAsync(
        Guid memberId,
        Guid requestingUserId)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member id cannot be empty.");

        return await _reminderRepository.GetByMemberAsync(memberId);
    }
}