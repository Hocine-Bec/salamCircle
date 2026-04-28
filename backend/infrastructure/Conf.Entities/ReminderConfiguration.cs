// ReminderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service.entities;

namespace infrastructure.configurations;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("Reminders");

        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.Circle)
            .WithMany()
            .HasForeignKey(r => r.CircleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.SentBy)
            .WithMany(u => u.SentReminders)
            .HasForeignKey(r => r.SentById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.SentTo)
            .WithMany()
            .HasForeignKey(r => r.SentToId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Cycle)
            .WithMany(c => c.Reminders)
            .HasForeignKey(r => r.CycleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}