using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    private const int EnumMaxLength = 16;

    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);
        builder.Property(order => order.Id).ValueGeneratedNever();
        builder.Property(order => order.Channel).HasConversion<string>().HasMaxLength(EnumMaxLength);
        builder.Property(order => order.Status).HasConversion<string>().HasMaxLength(EnumMaxLength);
        builder.Property(order => order.Method).HasConversion<string>().HasMaxLength(EnumMaxLength);
        builder.Property(order => order.Notes).HasMaxLength(Order.NotesMaxLength);
        builder.Ignore(order => order.Total);
        builder.Ignore(order => order.RefundedAmount);
        builder.Ignore(order => order.HasProducts);
        builder.Ignore(order => order.AwaitsPickup);

        builder.HasMany(order => order.Lines)
            .WithOne()
            .HasForeignKey(line => line.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(order => order.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Business>().WithMany().HasForeignKey(order => order.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Client>().WithMany().HasForeignKey(order => order.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(order => new { order.TenantId, order.CreatedAt });
        builder.HasIndex(order => new { order.TenantId, order.ClientId });
        builder.HasIndex(order => new { order.TenantId, order.Status });
    }
}
