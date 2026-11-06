using System.Collections.Frozen;

namespace ClassManager.Core.Domain.Countries;

public static class EuropeanCountries
{
    public static IReadOnlySet<string> Codes { get; } = new[]
    {
        "AT", "BE", "BG", "CY", "CZ", "DE", "DK", "EE", "ES", "FI", "FR", "GR", "HR", "HU", "IE", "IT", "LT", "LU", "LV", "MT",
        "NL", "PL", "PT", "RO", "SE", "SI", "SK", "IS", "LI", "NO",
    }.ToFrozenSet(StringComparer.Ordinal);
}
