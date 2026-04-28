using Microsoft.EntityFrameworkCore;
using infrastructure.data;
using service.entities;
using service.interfaces.repositories;

namespace infrastructure.repositories;

public class CircleInvitationRepository : ICircleInvitationRepository
{
    private readonly AppDbContext _context;

    public CircleInvitationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CircleInvitation?> GetByIdAsync(Guid id)
        => await _context.CircleInvitations
            .Include(i => i.Circle)
            .Include(i => i.InvitedBy)
            .Include(i => i.InvitedUser)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<List<CircleInvitation>> GetPendingByUserIdAsync(Guid userId)
        => await _context.CircleInvitations
            .Where(i => i.InvitedUserId == userId &&
                        i.Status == service.enums.InvitationStatus.Pending)
            .ToListAsync();

    public async Task<bool> ExistsAsync(Guid circleId, Guid userId)
        => await _context.CircleInvitations
            .AnyAsync(i => i.CircleId == circleId &&
                          i.InvitedUserId == userId);

    public async Task<CircleInvitation> CreateAsync(CircleInvitation invitation)
    {
        _context.CircleInvitations.Add(invitation);
        await _context.SaveChangesAsync();
        return invitation;
    }

    public async Task<CircleInvitation> UpdateAsync(CircleInvitation invitation)
    {
        _context.CircleInvitations.Update(invitation);
        await _context.SaveChangesAsync();
        return invitation;
    }

    public async Task DeleteAsync(Guid id)
    {
        var invitation = await _context.CircleInvitations
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invitation is not null)
        {
            _context.CircleInvitations.Remove(invitation);
            await _context.SaveChangesAsync();
        }
    }
}