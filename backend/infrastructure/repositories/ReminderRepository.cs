using Microsoft.EntityFrameworkCore;
using infrastructure.data;
using service.entities;
using service.interfaces.repositories;

namespace infrastructure.repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _context;

    public ReminderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Reminder?> GetByIdAsync(Guid id)
        => await _context.Reminders
            .Include(r => r.Circle)
            .Include(r => r.SentBy)
            .Include(r => r.SentTo)
            .Include(r => r.Cycle)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<Reminder>> GetByCycleAsync(Guid cycleId)
        => await _context.Reminders
            .Where(r => r.CycleId == cycleId)
            .ToListAsync();

    public async Task<List<Reminder>> GetByMemberAsync(Guid memberId)
        => await _context.Reminders
            .Where(r => r.SentToId == memberId)
            .ToListAsync();

    public async Task<int> CountByCycleAndMemberAsync(Guid cycleId, Guid memberId)
        => await _context.Reminders
            .CountAsync(r =>
                r.CycleId == cycleId &&
                r.SentToId == memberId);

    public async Task<Reminder> CreateAsync(Reminder reminder)
    {
        _context.Reminders.Add(reminder);
        await _context.SaveChangesAsync();
        return reminder;
    }

    public async Task DeleteAsync(Guid id)
    {
        var reminder = await _context.Reminders
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reminder is not null)
        {
            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
        }
    }
}