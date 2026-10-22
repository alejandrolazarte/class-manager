using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(variant => variant.Id);
        builder.Property(variant => variant.Size).HasMaxLength(ProductVariant.SizeMaxLength).IsRequired();
        builder.Property(variant => variant.Color).HasMaxLength(ProductColors.KeyMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(variant => variant.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(variant => new { variant.TenantId, variant.ProductId });
    }
}
