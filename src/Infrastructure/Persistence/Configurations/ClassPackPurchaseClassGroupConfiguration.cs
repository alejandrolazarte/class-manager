using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassPackPurchaseClassGroupConfiguration : IEntityTypeConfiguration<ClassPackPurchaseClassGroup>
{
    public void Configure(EntityTypeBuilder<ClassPackPurchaseClassGroup> builder)
    {
        builder.HasKey(purchaseClassGroup => new { purchaseClassGroup.ClassPackPurchaseId, purchaseClassGroup.ClassGroupId });

        builder.HasOne<Business>().WithMany().HasForeignKey(purchaseClassGroup => purchaseClassGroup.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassGroup>().WithMany().HasForeignKey(purchaseClassGroup => purchaseClassGroup.ClassGroupId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(purchaseClassGroup => new { purchaseClassGroup.TenantId, purchaseClassGroup.ClassGroupId });
    }
}
