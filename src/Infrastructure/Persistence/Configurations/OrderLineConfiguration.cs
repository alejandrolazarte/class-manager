using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    private const int KindMaxLength = 16;

    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.HasKey(line => line.Id);
        builder.Property(line => line.Id).ValueGeneratedNever();
        builder.Property(line => line.Kind).HasConversion<string>().HasMaxLength(KindMaxLength);
        builder.Property(line => line.Name).HasMaxLength(OrderLine.NameMaxLength).IsRequired();
        builder.Property(line => line.UnitPrice).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Property(line => line.RefundedAmount).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Ignore(line => line.Total);
        builder.Ignore(line => line.IsFullyRefunded);

        builder.HasOne<Business>().WithMany().HasForeignKey(line => line.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassPack>().WithMany().HasForeignKey(line => line.ClassPackId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassPackPurchase>().WithMany().HasForeignKey(line => line.ClassPackPurchaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Product>().WithMany().HasForeignKey(line => line.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ProductVariant>().WithMany().HasForeignKey(line => line.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(line => new { line.TenantId, line.OrderId });
        builder.HasIndex(line => new { line.TenantId, line.ClassPackPurchaseId });
    }
}
