using System.Collections.Frozen;

namespace ClassManager.Core.Domain.Subscriptions;

public static class SelfServiceBilling
{
    public const string Currency = "EUR";

    public static IReadOnlyList<string> PlanCodes { get; } = [Subscriptions.PlanCodes.Lite, Subscriptions.PlanCodes.Pro];

    public static IReadOnlySet<string> CountryCodes { get; } = new[]
    {
        "AT", "BE", "BG", "CY", "CZ", "DE", "DK", "EE", "ES", "FI", "FR", "GR", "HR", "HU", "IE", "IT", "LT", "LU", "LV", "MT",
        "NL", "PL", "PT", "RO", "SE", "SI", "SK", "IS", "LI", "NO",
    }.ToFrozenSet(StringComparer.Ordinal);
}
