// ContributionCycleConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service.entities;

namespace infrastructure.configurations;

public class ContributionCycleConfiguration : IEntityTypeConfiguration<ContributionCycle>
{
    public void Configure(EntityTypeBuilder<ContributionCycle> builder)
    {
        builder.ToTable("ContributionCycles");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CycleNumber)
            .IsRequired();

        builder.Property(c => c.Month)
            .IsRequired();

        builder.Property(c => c.Year)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired();

        builder.HasOne(c => c.Circle)
            .WithMany(c => c.Cycles)
            .HasForeignKey(c => c.CircleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}