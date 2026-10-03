using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Subscriptions.Subscribers;

public sealed class SubscriptionFeature
{
    private SubscriptionFeature()
    {
    }

    public Guid Id { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public string FeatureCode { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public int? Limit { get; private set; }
    public DateOnly StartsOn { get; private set; }
    public DateOnly? EndsOn { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static SubscriptionFeature Create(
        Guid subscriptionId,
        string featureCode,
        decimal price,
        string currency,
        int? limit,
        DateOnly startsOn,
        DateOnly? endsOn,
        DateTimeOffset createdAt)
    {
        if (endsOn < startsOn)
        {
            throw new ArgumentOutOfRangeException(nameof(endsOn), "A feature can't end before it starts.");
        }

        return new SubscriptionFeature
        {
            Id = Guid.CreateVersion7(),
            SubscriptionId = subscriptionId,
            FeatureCode = CatalogRules.RequireCode(featureCode, nameof(featureCode)),
            Price = CatalogRules.RequirePrice(price, nameof(price)),
            Currency = CatalogRules.RequireCurrency(currency, nameof(currency)),
            Limit = CatalogRules.RequireLimit(limit, nameof(limit)),
            StartsOn = startsOn,
            EndsOn = endsOn,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public bool IsActiveOn(DateOnly date) => StartsOn <= date && (EndsOn is null || date <= EndsOn);
}
