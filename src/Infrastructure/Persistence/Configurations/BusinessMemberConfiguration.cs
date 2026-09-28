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
        builder.HasIndex(member => new { member.TenantId, member.UserId }).IsUnique();
        builder.HasIndex(member => member.UserId);
        builder.HasOne<Instructor>().WithMany().HasForeignKey(member => member.InstructorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(member => new { member.TenantId, member.InstructorId }).IsUnique().HasFilter("[InstructorId] IS NOT NULL");
    }
}
