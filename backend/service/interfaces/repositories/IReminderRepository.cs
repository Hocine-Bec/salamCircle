using service.entities;

namespace service.interfaces.repositories;

public interface IReminderRepository
{
    Task<Reminder?> GetByIdAsync(Guid id);
    Task<List<Reminder>> GetByCycleAsync(Guid cycleId);
    Task<List<Reminder>> GetByMemberAsync(Guid memberId);
    Task<int> CountByCycleAndMemberAsync(Guid cycleId, Guid memberId);
    Task<Reminder> CreateAsync(Reminder reminder);
    Task DeleteAsync(Guid id);
}
