using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassManager.Infrastructure.Persistence.Configurations;

internal sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public const string TimeZonesTable = "CountryTimeZones";
    public const string CountryCodeColumn = "CountryCode";

    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(country => country.Code);
        builder.Property(country => country.Code).HasMaxLength(Country.CodeLength).IsFixedLength();
        builder.Property(country => country.CallingCode).HasMaxLength(Country.CallingCodeMaxLength).IsRequired();
        builder.Property(country => country.CurrencyCode).HasMaxLength(Business.CurrencyCodeLength).IsFixedLength().IsRequired();
        builder.OwnsMany(
            country => country.TimeZones,
            timeZone =>
            {
                timeZone.ToTable(TimeZonesTable);
                timeZone.WithOwner().HasForeignKey(CountryCodeColumn);
                timeZone.Property<string>(CountryCodeColumn).HasMaxLength(Country.CodeLength).IsFixedLength();
                timeZone.Property(countryTimeZone => countryTimeZone.TimeZoneId).HasMaxLength(Business.TimeZoneIdMaxLength);
                timeZone.HasKey(CountryCodeColumn, nameof(CountryTimeZone.TimeZoneId));
            });
        builder.Navigation(country => country.TimeZones).HasField("_timeZones");
    }
}
