using service.entities;

namespace service.interfaces.services;

public interface IReminderService
{
    Task SendReminderAsync(Guid cycleId, Guid targetMemberId, Guid imamId);
    Task<List<Reminder>> GetMemberRemindersAsync(Guid memberId, Guid requestingUserId);
}
