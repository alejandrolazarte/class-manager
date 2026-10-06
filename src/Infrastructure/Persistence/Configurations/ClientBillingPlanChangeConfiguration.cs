using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClientBillingPlanChangeConfiguration : IEntityTypeConfiguration<ClientBillingPlanChange>
{
    private const int KindMaxLength = 16;

    public void Configure(EntityTypeBuilder<ClientBillingPlanChange> builder)
    {
        builder.HasKey(change => change.Id);
        builder.Property(change => change.Id).ValueGeneratedNever();
        builder.Property(change => change.Kind).HasConversion<string>().HasMaxLength(KindMaxLength);
        builder.Property(change => change.CustomFee).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Ignore(change => change.Plan);

        builder.HasOne<Business>().WithMany().HasForeignKey(change => change.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Client>().WithMany().HasForeignKey(change => change.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(change => new { change.TenantId, change.ClientId, change.EffectiveFrom }).IsUnique().HasFilter(CurrentRecordIndex.Filter);
    }
}
