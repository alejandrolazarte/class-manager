namespace ClassManager.Infrastructure.Persistence;

internal static class CountryCatalogSeed
{
    private const int ArgentinaOwnAccountMinimumAge = 13;
    private const int SpainOwnAccountMinimumAge = 14;
    private const int GdprMaximumOwnAccountMinimumAge = 16;

    public static IReadOnlyList<Country> Countries { get; } =
    [
        Country.Create("AR", 1, "54", "ARS", ArgentinaOwnAccountMinimumAge, "America/Argentina/Buenos_Aires"),
        Country.Create("UY", 2, "598", "UYU", GdprMaximumOwnAccountMinimumAge, "America/Montevideo"),
        Country.Create("CL", 3, "56", "CLP", GdprMaximumOwnAccountMinimumAge, "America/Santiago", "Pacific/Easter"),
        Country.Create("PY", 4, "595", "PYG", GdprMaximumOwnAccountMinimumAge, "America/Asuncion"),
        Country.Create("BO", 5, "591", "BOB", GdprMaximumOwnAccountMinimumAge, "America/La_Paz"),
        Country.Create("PE", 6, "51", "PEN", GdprMaximumOwnAccountMinimumAge, "America/Lima"),
        Country.Create("EC", 7, "593", "USD", GdprMaximumOwnAccountMinimumAge, "America/Guayaquil", "Pacific/Galapagos"),
        Country.Create("CO", 8, "57", "COP", GdprMaximumOwnAccountMinimumAge, "America/Bogota"),
        Country.Create("VE", 9, "58", "VES", GdprMaximumOwnAccountMinimumAge, "America/Caracas"),
        Country.Create("PA", 10, "507", "USD", GdprMaximumOwnAccountMinimumAge, "America/Panama"),
        Country.Create("CR", 11, "506", "CRC", GdprMaximumOwnAccountMinimumAge, "America/Costa_Rica"),
        Country.Create("GT", 12, "502", "GTQ", GdprMaximumOwnAccountMinimumAge, "America/Guatemala"),
        Country.Create(
            "MX", 13, "52", "MXN", GdprMaximumOwnAccountMinimumAge,
            "America/Mexico_City", "America/Cancun", "America/Mazatlan", "America/Hermosillo", "America/Tijuana"),
        Country.Create("ES", 14, "34", "EUR", SpainOwnAccountMinimumAge, "Europe/Madrid", "Atlantic/Canary"),
    ];

    public static ModelBuilder SeedCountryCatalog(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>().HasData(Countries.Select(country => new
        {
            country.Code,
            country.DisplayOrder,
            country.CallingCode,
            country.CurrencyCode,
            country.OwnAccountMinimumAge,
        }));
        modelBuilder.Entity<Country>().OwnsMany(country => country.TimeZones).HasData(
            Countries.SelectMany(country => country.TimeZones.Select(timeZone => new
            {
                CountryCode = country.Code,
                timeZone.TimeZoneId,
                timeZone.DisplayOrder,
            })));
        return modelBuilder;
    }
}
