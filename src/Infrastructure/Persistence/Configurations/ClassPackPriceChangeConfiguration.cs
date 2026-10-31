using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class ClassPackPriceChangeConfiguration : IEntityTypeConfiguration<ClassPackPriceChange>
{
    public void Configure(EntityTypeBuilder<ClassPackPriceChange> builder)
    {
        builder.HasKey(change => change.Id);
        builder.Property(change => change.Price).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);

        builder.HasOne<Business>().WithMany().HasForeignKey(change => change.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ClassPack>().WithMany().HasForeignKey(change => change.ClassPackId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(change => new { change.TenantId, change.ClassPackId, change.EffectiveFrom }).IsUnique().HasFilter(SoftDeleteModelBuilderExtensions.NotDeletedFilter);
    }
}
