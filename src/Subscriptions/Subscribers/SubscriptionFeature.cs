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
    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }
    public DateOnly? ExpiredOn { get; private set; }

    public bool IsCurrent => DeletedOn is null;

    public static SubscriptionFeature Create(
        Guid subscriptionId,
        string featureCode,
        decimal price,
        string currency,
        int? limit,
        DateOnly? expiredOn,
        DateTimeOffset createdOn)
    {
        if (expiredOn < DateOnly.FromDateTime(createdOn.UtcDateTime))
        {
            throw new ArgumentOutOfRangeException(nameof(expiredOn), "A feature can't expire before it is created.");
        }

        return new SubscriptionFeature
        {
            Id = Guid.CreateVersion7(),
            SubscriptionId = subscriptionId,
            FeatureCode = CatalogRules.RequireCode(featureCode, nameof(featureCode)),
            Price = CatalogRules.RequirePrice(price, nameof(price)),
            Currency = CatalogRules.RequireCurrency(currency, nameof(currency)),
            Limit = CatalogRules.RequireLimit(limit, nameof(limit)),
            ExpiredOn = expiredOn,
            CreatedOn = createdOn.ToUniversalTime(),
        };
    }

    public void Delete(DateTimeOffset deletedOn)
    {
        if (DeletedOn is not null)
        {
            throw new InvalidOperationException("The feature was already replaced.");
        }

        DeletedOn = deletedOn.ToUniversalTime();
    }

    public bool IsActiveOn(DateOnly date) => IsCurrent && (ExpiredOn is null || date <= ExpiredOn);
}
