using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    private const int StockModeMaxLength = 24;

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).ValueGeneratedNever();
        builder.Property(product => product.Name).HasMaxLength(Product.NameMaxLength).IsRequired();
        builder.Property(product => product.Description).HasMaxLength(Product.DescriptionMaxLength);
        builder.Property(product => product.Price).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Property(product => product.StockMode).HasConversion<string>().HasMaxLength(StockModeMaxLength);
        builder.Ignore(product => product.TracksStock);

        builder.HasMany(product => product.Variants)
            .WithOne()
            .HasForeignKey(variant => variant.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(product => product.Variants).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Business>().WithMany().HasForeignKey(product => product.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(product => new { product.TenantId, product.Name }).IsUnique();
    }
}
