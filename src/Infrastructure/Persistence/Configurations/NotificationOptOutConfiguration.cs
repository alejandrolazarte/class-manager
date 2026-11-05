using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class NotificationOptOutConfiguration : IEntityTypeConfiguration<NotificationOptOut>
{
    private const int EnumMaxLength = 32;

    public void Configure(EntityTypeBuilder<NotificationOptOut> builder)
    {
        builder.HasKey(optOut => new { optOut.AccountId, optOut.Category, optOut.Channel });
        builder.Property(optOut => optOut.Category).HasConversion<string>().HasMaxLength(EnumMaxLength);
        builder.Property(optOut => optOut.Channel).HasConversion<string>().HasMaxLength(EnumMaxLength);
    }
}
