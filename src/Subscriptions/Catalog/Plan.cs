namespace ClassManager.Subscriptions.Catalog;

public sealed class Plan
{
    private Plan()
    {
    }

    public string Code { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public decimal? ListPrice { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public BillingPeriod BillingPeriod { get; private set; }
    public int? DurationInDays { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }

    public static Plan Create(
        string code,
        int displayOrder,
        decimal? listPrice,
        string currency,
        BillingPeriod billingPeriod,
        bool isDefault = false,
        int? durationInDays = null)
    {
        if (durationInDays is { } days)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(days, nameof(durationInDays));
        }

        return new Plan
        {
            Code = CatalogRules.RequireCode(code, nameof(code)),
            DisplayOrder = displayOrder,
            ListPrice = listPrice is { } price ? CatalogRules.RequirePrice(price, nameof(listPrice)) : null,
            Currency = CatalogRules.RequireCurrency(currency, nameof(currency)),
            BillingPeriod = billingPeriod,
            DurationInDays = durationInDays,
            IsDefault = isDefault,
            IsActive = true,
        };
    }
}
