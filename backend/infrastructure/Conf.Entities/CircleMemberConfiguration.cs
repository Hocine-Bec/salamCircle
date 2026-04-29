// CircleMemberConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service.entities;

namespace infrastructure.configurations;

public class CircleMemberConfiguration : IEntityTypeConfiguration<CircleMember>
{
    public void Configure(EntityTypeBuilder<CircleMember> builder)
    {
        builder.ToTable("CircleMembers");

        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.CircleId)
            .IsRequired();

        builder.Property(cm => cm.UserId)
            .IsRequired();

        builder.Property(cm => cm.QueuePosition)
            .IsRequired();

        builder.Property(cm => cm.SwapCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cm => cm.HasPausedThisCycle)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(cm => cm.Status)
            .IsRequired();

        builder.Property(cm => cm.JoinedAt)
            .IsRequired();

        builder.Property(cm => cm.RemovedAt)
            .IsRequired(false);

        builder.HasOne(cm => cm.Circle)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.CircleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cm => cm.User)
            .WithMany(u => u.CircleMemberships)
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(cm => new { cm.CircleId, cm.UserId })
            .IsUnique();
    }
}
