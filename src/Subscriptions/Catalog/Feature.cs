namespace ClassManager.Subscriptions.Catalog;

public sealed class Feature
{
    private Feature()
    {
    }

    public string Code { get; private set; } = string.Empty;
    public bool IsCounted { get; private set; }
    public bool IsAddOn { get; private set; }
    public decimal? AddOnListPrice { get; private set; }
    public string? Currency { get; private set; }

    public static Feature Create(string code, bool isCounted) =>
        new()
        {
            Code = CatalogRules.RequireCode(code, nameof(code)),
            IsCounted = isCounted,
        };

    public static Feature CreateAddOn(string code, bool isCounted, decimal addOnListPrice, string currency) =>
        new()
        {
            Code = CatalogRules.RequireCode(code, nameof(code)),
            IsCounted = isCounted,
            IsAddOn = true,
            AddOnListPrice = CatalogRules.RequirePrice(addOnListPrice, nameof(addOnListPrice)),
            Currency = CatalogRules.RequireCurrency(currency, nameof(currency)),
        };
}
