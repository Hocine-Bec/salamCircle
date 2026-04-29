// ContributionCycleRepository.cs
using Microsoft.EntityFrameworkCore;
using infrastructure.data;
using service.entities;
using service.interfaces.repositories;
using service.enums;

namespace infrastructure.repositories;

public class ContributionCycleRepository : IContributionCycleRepository
{
    private readonly AppDbContext _context;

    public ContributionCycleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ContributionCycle?> GetByIdAsync(Guid id)
        => await _context.ContributionCycles
            .Include(c => c.Circle)
            .Include(c => c.Contributions)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<ContributionCycle?> GetActiveByCircleIdAsync(Guid circleId)
        => await _context.ContributionCycles
            .FirstOrDefaultAsync(c =>
                c.CircleId == circleId &&
                c.Status == CycleStatus.Active);

    public async Task<ContributionCycle?> GetByCircleAndMonthAsync(Guid circleId, int month, int year)
        => await _context.ContributionCycles
            .FirstOrDefaultAsync(c =>
                c.CircleId == circleId &&
                c.Month == month &&
                c.Year == year);

    public async Task<List<ContributionCycle>> GetByCircleIdAsync(Guid circleId)
        => await _context.ContributionCycles
            .Where(c => c.CircleId == circleId)
            .OrderBy(c => c.CycleNumber)
            .ToListAsync();

    public async Task<ContributionCycle> CreateAsync(ContributionCycle cycle)
    {
        _context.ContributionCycles.Add(cycle);
        await _context.SaveChangesAsync();
        return cycle;
    }

    public async Task<ContributionCycle> UpdateAsync(ContributionCycle cycle)
    {
        _context.ContributionCycles.Update(cycle);
        await _context.SaveChangesAsync();
        return cycle;
    }
}