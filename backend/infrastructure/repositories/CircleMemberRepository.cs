using Microsoft.EntityFrameworkCore;
using infrastructure.data;
using service.entities;
using service.enums;
using service.interfaces.repositories;

namespace infrastructure.repositories;

public class CircleMemberRepository : ICircleMemberRepository
{
    private readonly AppDbContext _context;

    public CircleMemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CircleMember?> GetByIdAsync(Guid id)
        => await _context.CircleMembers.FirstOrDefaultAsync(cm => cm.Id == id);

    public async Task<CircleMember?> GetByCircleAndUserAsync(Guid circleId, Guid userId)
        => await _context.CircleMembers.FirstOrDefaultAsync(cm => cm.CircleId == circleId
                                              && cm.UserId == userId);

    public async Task<List<CircleMember>> GetByCircleIdAsync(Guid circleId)
        => await _context.CircleMembers.Where(cm => cm.CircleId == circleId).ToListAsync();

    public async Task<int> GetMaxQueuePositionAsync(Guid circleId)
        => await _context.CircleMembers.Where(cm => cm.CircleId == circleId).MaxAsync(cm => (int?)cm.QueuePosition) ?? 0;

    public async Task<int> CountActiveAsync(Guid circleId)
        => await _context.CircleMembers.CountAsync(cm => cm.CircleId == circleId && cm.Status == MemberStatus.Active);

    public async Task<bool> IsMemberAsync(Guid circleId, Guid userId)
        => await _context.CircleMembers.AnyAsync(cm => cm.CircleId == circleId && cm.UserId == userId);

    public async Task<CircleMember> CreateAsync(CircleMember member)
    {
        _context.CircleMembers.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<CircleMember> UpdateAsync(CircleMember member)
    {
        _context.CircleMembers.Update(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task DeleteAsync(Guid id)
    {
        var member = await _context.CircleMembers.FirstOrDefaultAsync(cm => cm.Id == id);
        if (member is not null)
        {
            _context.CircleMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}
