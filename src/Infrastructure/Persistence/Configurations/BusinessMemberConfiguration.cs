using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class BusinessMemberConfiguration : IEntityTypeConfiguration<BusinessMember>
{
    public void Configure(EntityTypeBuilder<BusinessMember> builder)
    {
        builder.HasKey(member => member.Id);
        builder.Property(member => member.Id).ValueGeneratedNever();
        builder.Property(member => member.Role)
            .HasConversion<string>()
            .HasMaxLength(BusinessMember.RoleMaxLength)
            .IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(member => member.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(member => new { member.TenantId, member.UserId }).IsUnique().HasFilter(SoftDeleteModelBuilderExtensions.NotDeletedFilter);
        builder.HasIndex(member => member.UserId);
        builder.HasOne<Instructor>().WithMany().HasForeignKey(member => member.InstructorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CustomRole>().WithMany().HasForeignKey(member => member.CustomRoleId).OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(table => table.HasCheckConstraint(
            "CK_BusinessMembers_CustomRoleId",
            "([Role] = 'Custom' AND [CustomRoleId] IS NOT NULL) OR ([Role] <> 'Custom' AND [CustomRoleId] IS NULL)"));
        builder.HasIndex(member => new { member.TenantId, member.InstructorId }).IsUnique().HasFilter($"[InstructorId] IS NOT NULL AND {SoftDeleteModelBuilderExtensions.NotDeletedFilter}");
    }
}
