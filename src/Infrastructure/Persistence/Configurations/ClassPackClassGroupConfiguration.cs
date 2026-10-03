using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassPackClassGroupConfiguration : IEntityTypeConfiguration<ClassPackClassGroup>
{
    public void Configure(EntityTypeBuilder<ClassPackClassGroup> builder)
    {
        builder.HasKey(classPackClassGroup => new { classPackClassGroup.ClassPackId, classPackClassGroup.ClassGroupId });

        builder.HasOne<Business>().WithMany().HasForeignKey(classPackClassGroup => classPackClassGroup.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassGroup>().WithMany().HasForeignKey(classPackClassGroup => classPackClassGroup.ClassGroupId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(classPackClassGroup => new { classPackClassGroup.TenantId, classPackClassGroup.ClassGroupId });
    }
}
