using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(variant => variant.Id);
        builder.Property(variant => variant.Id).ValueGeneratedNever();
        builder.Property(variant => variant.Name).HasMaxLength(ProductVariant.NameMaxLength).IsRequired();

        builder.HasOne<Business>().WithMany().HasForeignKey(variant => variant.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(variant => new { variant.TenantId, variant.ProductId });
    }
}
