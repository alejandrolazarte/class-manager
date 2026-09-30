using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public const string RetiredDefaultMonthlyFeeColumn = "DefaultMonthlyFee";

    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.HasKey(business => business.Id);
        builder.Property<decimal?>(RetiredDefaultMonthlyFeeColumn).HasPrecision(MonthlyFee.AmountPrecision, MonthlyFee.AmountDecimals);
        builder.Property(business => business.Id).ValueGeneratedNever();
        builder.Property(business => business.Name).HasMaxLength(Business.NameMaxLength).IsRequired();
        builder.Property(business => business.Slug).HasMaxLength(Business.SlugMaxLength).IsRequired();
        builder.Property(business => business.TimeZoneId).HasMaxLength(Business.TimeZoneIdMaxLength).IsRequired();
        builder.Property(business => business.CurrencyCode).HasMaxLength(Business.CurrencyCodeLength).IsFixedLength().IsRequired();
        builder.Property(business => business.DefaultCountryCallingCode).HasMaxLength(Business.DefaultCountryCallingCodeMaxLength).IsRequired();
        builder.Property(business => business.BrandName).HasMaxLength(Business.NameMaxLength);
        builder.Property(business => business.ThemeColor).HasMaxLength(Business.BrandColorLength).IsFixedLength();
        builder.Property(business => business.AccentColor).HasMaxLength(Business.BrandColorLength).IsFixedLength();
        builder.Ignore(business => business.BrandDisplayName);
        builder.HasIndex(business => business.Slug).IsUnique();
        builder.HasOne<Organization>().WithMany().HasForeignKey(business => business.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
