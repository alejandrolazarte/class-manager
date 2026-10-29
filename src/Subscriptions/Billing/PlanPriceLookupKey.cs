namespace ClassManager.Subscriptions.Billing;

public static class PlanPriceLookupKey
{
    public const string Monthly = "monthly";

    private const char Separator = '-';
    private const int PartCount = 3;

    public static string For(string planCode, string currency) =>
        string.Join(Separator, planCode, Monthly, currency.ToLowerInvariant());

    public static string? PlanCodeOf(string? lookupKey)
    {
        var parts = lookupKey?.Split(Separator);
        return parts is { Length: PartCount } && parts[1] == Monthly && parts[0].Length > 0
            ? parts[0]
            : null;
    }
}
