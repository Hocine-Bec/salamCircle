using Microsoft.EntityFrameworkCore;
using infrastructure.data;
using service.entities;
using service.interfaces.repositories;

namespace infrastructure.repositories;

public class CircleRepository : ICircleRepository
{
    private readonly AppDbContext _context;

    public CircleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Circle?> GetByIdAsync(Guid id)
        => await _context.Circles
            .Include(c => c.Imam)
            .Include(c => c.Members)
            .Include(c => c.Cycles)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Circle>> GetByUserIdAsync(Guid userId)
        => await _context.Circles
            .Include(c => c.Imam)
            .Include(c => c.Members)
            .Where(c => c.ImamId == userId ||
                        c.Members.Any(m => m.UserId == userId))
            .ToListAsync();

    public async Task<Circle> CreateAsync(Circle circle)
    {
        _context.Circles.Add(circle);
        await _context.SaveChangesAsync();
        return circle;
    }

    public async Task<Circle> UpdateAsync(Circle circle)
    {
        _context.Circles.Update(circle);
        await _context.SaveChangesAsync();
        return circle;
    }
}