namespace ClassManager.Core.Domain.Countries;

public sealed class Country
{
    public const int CodeLength = 2;
    public const int CallingCodeMaxLength = 3;

    private readonly List<CountryTimeZone> _timeZones = [];

    private Country()
    {
    }

    public string Code { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public string CallingCode { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public int OwnAccountMinimumAge { get; private set; }
    public IReadOnlyList<CountryTimeZone> TimeZones => _timeZones;

    public static Country Create(
        string code,
        int displayOrder,
        string callingCode,
        string currencyCode,
        int ownAccountMinimumAge,
        params string[] timeZoneIds)
    {
        var country = new Country
        {
            Code = code,
            DisplayOrder = displayOrder,
            CallingCode = callingCode,
            CurrencyCode = currencyCode,
            OwnAccountMinimumAge = ownAccountMinimumAge,
        };
        country._timeZones.AddRange(timeZoneIds.Select((timeZoneId, index) => new CountryTimeZone(timeZoneId, index + 1)));
        return country;
    }

    public bool HasTimeZone(string? timeZoneId) => _timeZones.Any(timeZone => timeZone.TimeZoneId == timeZoneId);
}
