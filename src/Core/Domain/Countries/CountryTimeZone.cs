namespace ClassManager.Core.Domain.Countries;

public sealed class CountryTimeZone
{
    public CountryTimeZone(string timeZoneId, int displayOrder)
    {
        TimeZoneId = timeZoneId;
        DisplayOrder = displayOrder;
    }

    public string TimeZoneId { get; private set; }
    public int DisplayOrder { get; private set; }
}
