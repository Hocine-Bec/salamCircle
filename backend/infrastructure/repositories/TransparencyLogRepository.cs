using Microsoft.EntityFrameworkCore;
using infrastructure.data;
using service.entities;
using service.enums;
using service.interfaces.repositories;

namespace infrastructure.repositories;

public class TransparencyLogRepository : ITransparencyLogRepository
{
    private readonly AppDbContext _context;

    public TransparencyLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransparencyLog>> GetByCircleIdAsync(Guid circleId)
        => await _context.TransparencyLogs
            .Where(log => log.CircleId == circleId)
            .OrderByDescending(log => log.CreatedAt)
            .ToListAsync();

    public async Task<List<TransparencyLog>> GetByCircleIdAndTypeAsync(Guid circleId, LogEventType eventType)
        => await _context.TransparencyLogs
            .Where(log => log.CircleId == circleId && log.EventType == eventType)
            .OrderByDescending(log => log.CreatedAt)
            .ToListAsync();

    public async Task<TransparencyLog> CreateAsync(TransparencyLog log)
    {
        _context.TransparencyLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }
}
