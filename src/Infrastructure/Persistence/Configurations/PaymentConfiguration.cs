using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    private const int MethodMaxLength = 16;

    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(payment => payment.Id);
        builder.Property(payment => payment.Id).ValueGeneratedNever();
        builder.Property(payment => payment.Amount).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Property(payment => payment.Method).HasConversion<string>().HasMaxLength(MethodMaxLength);
        builder.Property(payment => payment.Notes).HasMaxLength(Payment.NotesMaxLength);
        builder.Ignore(payment => payment.BillingMonth);

        builder.HasOne<Business>().WithMany().HasForeignKey(payment => payment.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Client>().WithMany().HasForeignKey(payment => payment.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(payment => new { payment.TenantId, payment.Month, payment.ClientId });
        builder.HasIndex(payment => new { payment.TenantId, payment.ClientId, payment.PaidOn });
    }
}
