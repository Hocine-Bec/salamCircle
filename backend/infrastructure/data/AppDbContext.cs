using Microsoft.EntityFrameworkCore;
using service.entities;

namespace infrastructure.data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Circle> Circles => Set<Circle>();
    public DbSet<CircleMember> CircleMembers => Set<CircleMember>();
    public DbSet<CircleInvitation> CircleInvitations => Set<CircleInvitation>();
    public DbSet<ContributionCycle> ContributionCycles => Set<ContributionCycle>();
    public DbSet<Contribution> Contributions => Set<Contribution>();
    public DbSet<EmergencyRequest> EmergencyRequests => Set<EmergencyRequest>();
    public DbSet<SwapRequest> SwapRequests => Set<SwapRequest>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<TransparencyLog> TransparencyLogs => Set<TransparencyLog>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically applies all IEntityTypeConfiguration<T> classes in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
