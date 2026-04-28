// CircleInvitationConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service.entities;

namespace infrastructure.configurations;

public class CircleInvitationConfiguration : IEntityTypeConfiguration<CircleInvitation>
{
    public void Configure(EntityTypeBuilder<CircleInvitation> builder)
    {
        builder.ToTable("CircleInvitations");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Status)
            .IsRequired();

        builder.HasOne(i => i.Circle)
            .WithMany(c => c.Invitations)
            .HasForeignKey(i => i.CircleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.InvitedBy)
            .WithMany(u => u.SentInvitations)
            .HasForeignKey(i => i.InvitedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.InvitedUser)
            .WithMany(u => u.ReceivedInvitations)
            .HasForeignKey(i => i.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}