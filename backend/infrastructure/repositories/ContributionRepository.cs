using Microsoft.EntityFrameworkCore;
using infrastructure.data;
using service.entities;
using service.enums;
using service.interfaces.repositories;

namespace infrastructure.repositories;

public class ContributionRepository : IContributionRepository
{
    private readonly AppDbContext _context;

    public ContributionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Contribution?> GetByIdAsync(Guid id)
        => await _context.Contributions
            .Include(c => c.Circle)
            .Include(c => c.Member)
            .Include(c => c.Cycle)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Contribution?> GetByCycleAndMemberAsync(Guid cycleId, Guid memberId)
        => await _context.Contributions
            .FirstOrDefaultAsync(c => c.CycleId == cycleId && c.MemberId == memberId);


    
    public async Task<List<Contribution>> GetByCircleAndMemberAsync(Guid circleId, Guid memberId)
        => await _context.Contributions
            .Where(c => c.CircleId == circleId && c.MemberId == memberId)
            .OrderByDescending(c => c.ContributedAt)
            .ToListAsync();


    public async Task<List<Contribution>> GetByCycleIdAsync(Guid cycleId)
        => await _context.Contributions
            .Where(c => c.CycleId == cycleId)
            .ToListAsync();

    public async Task<List<Contribution>> GetByMemberIdAsync(Guid memberId)
        => await _context.Contributions
            .Where(c => c.MemberId == memberId)
            .ToListAsync();

    public async Task<decimal> GetTotalByCircleIdAsync(Guid circleId)
        => await _context.Contributions
        .Where(c => c.CircleId == circleId && c.Status == ContributionStatus.Completed)
        .SumAsync(c => c.Amount); 

    public async Task<Contribution> CreateAsync(Contribution contribution)
    {
        _context.Contributions.Add(contribution);
        await _context.SaveChangesAsync();
        return contribution;
    }

    public async Task<Contribution> UpdateAsync(Contribution contribution)
    {
        _context.Contributions.Update(contribution);
        await _context.SaveChangesAsync();
        return contribution;
    }

    public async Task DeleteAsync(Guid id)
    {
        var contribution = await _context.Contributions.FirstOrDefaultAsync(x => x.Id == id);
        if (contribution is not null)
        {
            _context.Contributions.Remove(contribution);
            await _context.SaveChangesAsync();
        }
    }
}