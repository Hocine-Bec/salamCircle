// ContributionConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service.entities;

namespace infrastructure.configurations;

public class ContributionConfiguration : IEntityTypeConfiguration<Contribution>
{
    public void Configure(EntityTypeBuilder<Contribution> builder)
    {
        builder.ToTable("Contributions");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired();

        builder.HasOne(c => c.Cycle)
            .WithMany(cc => cc.Contributions)
            .HasForeignKey(c => c.CycleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Circle)
            .WithMany(c => c.Cycles.SelectMany(x => x.Contributions))
            .HasForeignKey(c => c.CircleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Member)
            .WithMany()
            .HasForeignKey(c => c.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}