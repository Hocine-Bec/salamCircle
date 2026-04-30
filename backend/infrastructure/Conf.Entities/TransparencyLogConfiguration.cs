// TransparencyLogConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service.entities;

namespace infrastructure.configurations;

public class TransparencyLogConfiguration : IEntityTypeConfiguration<TransparencyLog>
{
    public void Configure(EntityTypeBuilder<TransparencyLog> builder)
    {
        builder.ToTable("TransparencyLogs");

        builder.HasKey(tl => tl.Id);

        builder.Property(tl => tl.CircleId)
            .IsRequired();

        builder.Property(tl => tl.EventType)
            .IsRequired();

        builder.Property(tl => tl.ActorId)
            .IsRequired(false);

        builder.Property(tl => tl.TargetId)
            .IsRequired(false);

        builder.Property(tl => tl.ReferenceId)
            .IsRequired(false);

        builder.Property(tl => tl.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(tl => tl.Metadata)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.HasOne(tl => tl.Circle)
            .WithMany(c => c.TransparencyLogs)
            .HasForeignKey(tl => tl.CircleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tl => tl.Actor)
            .WithMany(u => u.ActorLogs)
            .HasForeignKey(tl => tl.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tl => tl.Target)
            .WithMany(u => u.TargetLogs)
            .HasForeignKey(tl => tl.TargetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
