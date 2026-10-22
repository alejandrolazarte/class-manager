using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class DefaultMonthlyFeeChangeConfiguration : IEntityTypeConfiguration<DefaultMonthlyFeeChange>
{
    private const int BillingModeMaxLength = 16;

    public void Configure(EntityTypeBuilder<DefaultMonthlyFeeChange> builder)
    {
        builder.HasKey(change => change.Id);
        builder.Property(change => change.Id).ValueGeneratedNever();
        builder.Property(change => change.BillingMode).HasConversion<string>().HasMaxLength(BillingModeMaxLength);
        builder.Property(change => change.Amount).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);

        builder.HasOne<Business>().WithMany().HasForeignKey(change => change.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(change => new { change.TenantId, change.EffectiveFrom }).IsUnique().HasFilter(SoftDeleteModelBuilderExtensions.NotDeletedFilter);
    }
}
