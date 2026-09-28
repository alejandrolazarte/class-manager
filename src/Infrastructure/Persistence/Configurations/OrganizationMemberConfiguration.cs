using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.HasKey(member => member.Id);
        builder.Property(member => member.Id).ValueGeneratedNever();
        builder.Property(member => member.Role)
            .HasConversion<string>()
            .HasMaxLength(OrganizationMember.RoleMaxLength)
            .IsRequired();

        builder.HasOne<Organization>().WithMany().HasForeignKey(member => member.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(member => new { member.OrganizationId, member.UserId }).IsUnique();
        builder.HasIndex(member => member.UserId);
    }
}
