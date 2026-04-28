// CircleConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service.entities;

namespace infrastructure.configurations;

public class CircleConfiguration : IEntityTypeConfiguration<Circle>
{
    public void Configure(EntityTypeBuilder<Circle> builder)
    {
        builder.ToTable("Circles");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.MinimumContribution)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.ContributorsPerMonth)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired();

        builder.HasOne(c => c.Imam)
            .WithMany(u => u.ImamOfCircles)
            .HasForeignKey(c => c.ImamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}