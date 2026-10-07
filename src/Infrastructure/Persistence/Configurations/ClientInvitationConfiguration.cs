using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClientInvitationConfiguration : IEntityTypeConfiguration<ClientInvitation>
{
    private const string GuardianTokenHashIsNotNullFilter = "[GuardianTokenHash] IS NOT NULL";

    public void Configure(EntityTypeBuilder<ClientInvitation> builder)
    {
        builder.HasKey(invitation => invitation.Id);
        builder.Property(invitation => invitation.Id).ValueGeneratedNever();
        builder.Property(invitation => invitation.Email).HasMaxLength(ClientInvitation.EmailMaxLength).IsRequired();
        builder.Property(invitation => invitation.TokenHash).HasMaxLength(ClientInvitation.TokenHashLength).IsFixedLength().IsRequired();
        builder.Property(invitation => invitation.GuardianEmail).HasMaxLength(ClientInvitation.EmailMaxLength);
        builder.Property(invitation => invitation.GuardianTokenHash).HasMaxLength(ClientInvitation.TokenHashLength).IsFixedLength();

        builder.HasOne<Business>().WithMany().HasForeignKey(invitation => invitation.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Client>().WithMany().HasForeignKey(invitation => invitation.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Student>().WithMany().HasForeignKey(invitation => invitation.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(invitation => invitation.TokenHash).IsUnique();
        builder.HasIndex(invitation => invitation.GuardianTokenHash).IsUnique().HasFilter(GuardianTokenHashIsNotNullFilter);
        builder.HasIndex(invitation => new { invitation.TenantId, invitation.ClientId });
    }
}
