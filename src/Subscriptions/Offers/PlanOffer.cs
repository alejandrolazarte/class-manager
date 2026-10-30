using ClassManager.Records;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Subscriptions.Offers;

public sealed class PlanOffer : ICreatedOn, ISoftDeletable, IExpiredOn
{
    private PlanOffer()
    {
    }

    public long Id { get; private set; }
    public long SubscriberId { get; private set; }
    public string PlanCode { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string? Note { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateOnly? ExpiredOn { get; private set; }

    public static PlanOffer Create(
        long subscriberId,
        string planCode,
        decimal price,
        string currency,
        string? note,
        DateOnly? expiredOn,
        DateTimeOffset createdOn)
    {
        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > CatalogRules.NoteMaxLength })
        {
            throw new ArgumentException($"Note must be at most {CatalogRules.NoteMaxLength} characters.", nameof(note));
        }

        if (expiredOn < DateOnly.FromDateTime(createdOn.UtcDateTime))
        {
            throw new ArgumentOutOfRangeException(nameof(expiredOn), "An offer can't expire before it is created.");
        }

        return new PlanOffer
        {
            SubscriberId = subscriberId,
            PlanCode = CatalogRules.RequireCode(planCode, nameof(planCode)),
            Price = CatalogRules.RequirePrice(price, nameof(price)),
            Currency = CatalogRules.RequireCurrency(currency, nameof(currency)),
            Note = trimmedNote,
            ExpiredOn = expiredOn,
            CreatedOn = createdOn.ToUniversalTime(),
        };
    }

    public bool IsPaidBy(Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        return subscription.SubscriberId == SubscriberId
            && subscription.ExternalSubscriptionId is not null
            && subscription.PlanCode == PlanCode
            && subscription.Price == Price
            && string.Equals(subscription.Currency, Currency, StringComparison.OrdinalIgnoreCase);
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
