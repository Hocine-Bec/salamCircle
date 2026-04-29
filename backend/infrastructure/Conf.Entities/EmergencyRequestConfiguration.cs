// EmergencyRequestConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service.entities;

namespace infrastructure.configurations;

public class EmergencyRequestConfiguration : IEntityTypeConfiguration<EmergencyRequest>
{
    public void Configure(EntityTypeBuilder<EmergencyRequest> builder)
    {
        builder.ToTable("EmergencyRequests");

        builder.HasKey(er => er.Id);

        builder.Property(er => er.CircleId)
            .IsRequired();

        builder.Property(er => er.RequestedById)
            .IsRequired();

        builder.Property(er => er.AmountRequested)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(er => er.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(er => er.SupportingContext)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(er => er.Status)
            .IsRequired();

        builder.Property(er => er.ReviewedById)
            .IsRequired(false);

        builder.Property(er => er.ReviewedAt)
            .IsRequired(false);

        builder.Property(er => er.RejectionReason)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasOne(er => er.Circle)
            .WithMany(c => c.EmergencyRequests)
            .HasForeignKey(er => er.CircleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(er => er.RequestedBy)
            .WithMany(cm => cm.EmergencyRequests)
            .HasForeignKey(er => er.RequestedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(er => er.ReviewedBy)
            .WithMany(u => u.ReviewedRequests)
            .HasForeignKey(er => er.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
