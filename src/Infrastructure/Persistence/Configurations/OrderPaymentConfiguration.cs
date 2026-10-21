using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class OrderPaymentConfiguration : IEntityTypeConfiguration<OrderPayment>
{
    private const int MethodMaxLength = 16;

    public void Configure(EntityTypeBuilder<OrderPayment> builder)
    {
        builder.HasKey(payment => payment.Id);
        builder.Property(payment => payment.Id).ValueGeneratedNever();
        builder.Property(payment => payment.Amount).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Property(payment => payment.Method).HasConversion<string>().HasMaxLength(MethodMaxLength);

        builder.HasOne<Business>().WithMany().HasForeignKey(payment => payment.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(payment => new { payment.TenantId, payment.OrderId });
        builder.HasIndex(payment => new { payment.TenantId, payment.PaidOn });
    }
}
