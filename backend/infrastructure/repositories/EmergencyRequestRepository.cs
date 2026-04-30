using Microsoft.EntityFrameworkCore;
using infrastructure.data;
using service.entities;
using service.enums;
using service.interfaces.repositories;

namespace infrastructure.repositories;

public class EmergencyRequestRepository : IEmergencyRequestRepository
{
    private readonly AppDbContext _context;

    public EmergencyRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmergencyRequest?> GetByIdAsync(Guid id)
        => await _context.EmergencyRequests.FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<EmergencyRequest>> GetByCircleIdAsync(Guid circleId)
        => await _context.EmergencyRequests
            .Where(r => r.CircleId == circleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<bool> HasPendingRequestAsync(Guid memberId)
        => await _context.EmergencyRequests.AnyAsync(r => r.RequestedById == memberId && r.Status == EmergencyStatus.Pending);

    public async Task<EmergencyRequest> CreateAsync(EmergencyRequest request)
    {
        _context.EmergencyRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<EmergencyRequest> UpdateAsync(EmergencyRequest request)
    {
        _context.EmergencyRequests.Update(request);
        await _context.SaveChangesAsync();
        return request;
    }

    // US-07: returns total amount disbursed from approved emergency requests
public async Task<decimal> GetTotalDisbursedAsync(Guid circleId)
    => await _context.EmergencyRequests
        .Where(r => r.CircleId == circleId && r.Status == EmergencyStatus.Approved)
        .SumAsync(r => (decimal?)r.AmountRequested) ?? 0m;

    public Task DeleteAsync(Guid id)
    => throw new NotSupportedException("Emergency requests cannot be deleted. They are permanent financial records.");
}
