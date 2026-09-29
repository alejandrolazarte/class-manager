using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    private const int KindMaxLength = 16;

    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.HasKey(movement => movement.Id);
        builder.Property(movement => movement.Id).ValueGeneratedNever();
        builder.Property(movement => movement.Kind).HasConversion<string>().HasMaxLength(KindMaxLength);
        builder.Property(movement => movement.Note).HasMaxLength(StockMovement.NoteMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(movement => movement.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ProductVariant>().WithMany().HasForeignKey(movement => movement.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Order>().WithMany().HasForeignKey(movement => movement.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(movement => new { movement.TenantId, movement.ProductVariantId });
    }
}
