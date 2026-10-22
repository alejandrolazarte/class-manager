using ClassManager.Records;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Subscriptions.Subscribers;

public sealed class SubscriptionFeature : ICreatedOn, ISoftDeletable, IExpiredOn
{
    private SubscriptionFeature()
    {
    }

    public long Id { get; private set; }
    public long SubscriptionId { get; private set; }
    public string FeatureCode { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public int? Limit { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateOnly? ExpiredOn { get; private set; }

    public static SubscriptionFeature Create(
        long subscriptionId,
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
            SubscriptionId = subscriptionId,
            FeatureCode = CatalogRules.RequireCode(featureCode, nameof(featureCode)),
            Price = CatalogRules.RequirePrice(price, nameof(price)),
            Currency = CatalogRules.RequireCurrency(currency, nameof(currency)),
            Limit = CatalogRules.RequireLimit(limit, nameof(limit)),
            ExpiredOn = expiredOn,
            CreatedOn = createdOn.ToUniversalTime(),
        };
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }

    public void Restore()
    {
        DeletedAt = null;
        DeletedByUserId = null;
        IsDeleted = false;
    }
}
