// SwapRequestConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service.entities;

namespace infrastructure.configurations;

public class SwapRequestConfiguration : IEntityTypeConfiguration<SwapRequest>
{
    public void Configure(EntityTypeBuilder<SwapRequest> builder)
    {
        builder.ToTable("SwapRequests");

        builder.HasKey(sr => sr.Id);

        builder.Property(sr => sr.CircleId)
            .IsRequired();

        builder.Property(sr => sr.CycleId)
            .IsRequired();

        builder.Property(sr => sr.RequesterId)
            .IsRequired();

        builder.Property(sr => sr.AcceptorId)
            .IsRequired(false);

        builder.Property(sr => sr.RequesterOriginalPosition)
            .IsRequired();

        builder.Property(sr => sr.AcceptorOriginalPosition)
            .IsRequired(false);

        builder.Property(sr => sr.Status)
            .IsRequired();

        builder.Property(sr => sr.AcceptedAt)
            .IsRequired(false);

        builder.HasOne(sr => sr.Requester)
            .WithMany(cm => cm.SwapRequestsAsRequester)
            .HasForeignKey(sr => sr.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.Acceptor)
            .WithMany(cm => cm.SwapRequestsAsAcceptor)
            .HasForeignKey(sr => sr.AcceptorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.Cycle)
            .WithMany(c => c.SwapRequests)
            .HasForeignKey(sr => sr.CycleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.Circle)
            .WithMany()
            .HasForeignKey(sr => sr.CircleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
