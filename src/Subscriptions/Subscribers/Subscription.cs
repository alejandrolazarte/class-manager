using ClassManager.Records;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Subscriptions.Subscribers;

public sealed class Subscription : ICreatedOn, ISoftDeletable, IExpiredOn
{
    public const int ExternalSubscriptionIdMaxLength = 255;

    private Subscription()
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
    public string? ExternalSubscriptionId { get; private set; }
    public SubscriptionPaymentStatus? PaymentStatus { get; private set; }

    public static Subscription StartAtListPrice(long subscriberId, Plan plan, DateTimeOffset createdOn)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var subscription = Start(subscriberId, plan.Code, plan.ListPrice ?? 0m, plan.Currency, note: null, createdOn);
        if (plan.DurationInDays is { } durationInDays)
        {
            subscription.Expire(subscription.CreatedDate().AddDays(durationInDays - 1));
        }

        return subscription;
    }

    public static Subscription Start(
        long subscriberId,
        string planCode,
        decimal price,
        string currency,
        string? note,
        DateTimeOffset createdOn)
    {
        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > CatalogRules.NoteMaxLength })
        {
            throw new ArgumentException($"Note must be at most {CatalogRules.NoteMaxLength} characters.", nameof(note));
        }

        return new Subscription
        {
            SubscriberId = subscriberId,
            PlanCode = CatalogRules.RequireCode(planCode, nameof(planCode)),
            Price = CatalogRules.RequirePrice(price, nameof(price)),
            Currency = CatalogRules.RequireCurrency(currency, nameof(currency)),
            Note = trimmedNote,
            CreatedOn = createdOn.ToUniversalTime(),
        };
    }

    public static Subscription StartExternal(
        long subscriberId,
        string planCode,
        decimal price,
        string currency,
        string externalSubscriptionId,
        DateTimeOffset createdOn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(externalSubscriptionId);
        if (externalSubscriptionId.Length > ExternalSubscriptionIdMaxLength)
        {
            throw new ArgumentException(
                $"External subscription id must be at most {ExternalSubscriptionIdMaxLength} characters.",
                nameof(externalSubscriptionId));
        }

        var subscription = Start(subscriberId, planCode, price, currency, note: null, createdOn);
        subscription.ExternalSubscriptionId = externalSubscriptionId;
        subscription.PaymentStatus = SubscriptionPaymentStatus.Current;
        return subscription;
    }

    public void Expire(DateOnly expiredOn)
    {
        if (expiredOn < this.CreatedDate())
        {
            throw new ArgumentOutOfRangeException(nameof(expiredOn), "A subscription can't expire before it starts.");
        }

        ExpiredOn = expiredOn;
    }

    public void ClearExpiry() => ExpiredOn = null;

    public void ChangePaymentStatus(SubscriptionPaymentStatus paymentStatus) => PaymentStatus = paymentStatus;

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
