using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class MemberInvitationConfiguration : IEntityTypeConfiguration<MemberInvitation>
{
    public void Configure(EntityTypeBuilder<MemberInvitation> builder)
    {
        builder.HasKey(invitation => invitation.Id);
        builder.Property(invitation => invitation.Id).ValueGeneratedNever();
        builder.Property(invitation => invitation.Email).HasMaxLength(MemberInvitation.EmailMaxLength).IsRequired();
        builder.Property(invitation => invitation.Role)
            .HasConversion<string>()
            .HasMaxLength(BusinessMember.RoleMaxLength)
            .IsRequired();
        builder.Property(invitation => invitation.TokenHash).HasMaxLength(MemberInvitation.TokenHashLength).IsFixedLength().IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(invitation => invitation.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Instructor>().WithMany().HasForeignKey(invitation => invitation.InstructorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(invitation => invitation.TokenHash).IsUnique();
        builder.HasIndex(invitation => new { invitation.TenantId, invitation.Email });
    }
}
