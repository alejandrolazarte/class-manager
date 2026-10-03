using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(image => new { image.ProductId, image.DocumentId });
        builder.HasOne(image => image.Document).WithMany().HasForeignKey(image => image.DocumentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(image => image.DocumentId).IsUnique();
        builder.Navigation(image => image.Document).AutoInclude();

        builder.HasOne<Business>().WithMany().HasForeignKey(image => image.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(image => image.TenantId);
    }
}
