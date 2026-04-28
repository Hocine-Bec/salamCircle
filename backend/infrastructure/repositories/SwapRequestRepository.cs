using Microsoft.EntityFrameworkCore;
using infrastructure.data;
using service.entities;
using service.enums;
using service.interfaces.repositories;

namespace infrastructure.repositories;

public class SwapRequestRepository : ISwapRequestRepository
{
    private readonly AppDbContext _context;

    public SwapRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SwapRequest?> GetByIdAsync(Guid id)
        => await _context.SwapRequests.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<SwapRequest>> GetOpenByCycleIdAsync(Guid cycleId)
        => await _context.SwapRequests
            .Where(s => s.CycleId == cycleId && s.Status == SwapStatus.Open)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();

    public async Task<int> CountByMemberAndCycleAsync(Guid memberId, Guid cycleId)
        => await _context.SwapRequests.CountAsync(s => s.RequesterId == memberId && s.CycleId == cycleId);

    public async Task<SwapRequest> CreateAsync(SwapRequest swapRequest)
    {
        _context.SwapRequests.Add(swapRequest);
        await _context.SaveChangesAsync();
        return swapRequest;
    }

    public async Task<SwapRequest> UpdateAsync(SwapRequest swapRequest)
    {
        _context.SwapRequests.Update(swapRequest);
        await _context.SaveChangesAsync();
        return swapRequest;
    }

    public async Task DeleteAsync(Guid id)
    {
        var swapRequest = await _context.SwapRequests.FirstOrDefaultAsync(s => s.Id == id);
        if (swapRequest is not null)
        {
            _context.SwapRequests.Remove(swapRequest);
            await _context.SaveChangesAsync();
        }
    }
}
