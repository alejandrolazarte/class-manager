using System.Collections.Frozen;
using ClassManager.Subscriptions.Billing;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Core.Domain.Subscriptions;

public static class SelfServiceBilling
{
    public const string Currency = "EUR";
    public const string ProductNamePrefix = "Laztty ";

    public static IReadOnlyList<string> PlanCodes { get; } = [Subscriptions.PlanCodes.Lite, Subscriptions.PlanCodes.Pro];

    public static IReadOnlySet<string> CountryCodes { get; } = new[]
    {
        "AT", "BE", "BG", "CY", "CZ", "DE", "DK", "EE", "ES", "FI", "FR", "GR", "HR", "HU", "IE", "IT", "LT", "LU", "LV", "MT",
        "NL", "PL", "PT", "RO", "SE", "SI", "SK", "IS", "LI", "NO",
    }.ToFrozenSet(StringComparer.Ordinal);

    public static BillingPlanPrice PriceOf(Plan plan, decimal unitAmount)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return new BillingPlanPrice(plan.Code, ProductNameOf(plan.Code), unitAmount, Currency);
    }

    private static string ProductNameOf(string planCode) =>
        ProductNamePrefix + char.ToUpperInvariant(planCode[0]) + planCode[1..];
}
