using ClassManager.Records;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Subscriptions.Subscribers;

public sealed class Subscription : ICreatedOn, IDeletedOn, IExpiredOn
{
    private Subscription()
    {
    }

    public Guid Id { get; private set; }
    public Guid SubscriberId { get; private set; }
    public string PlanCode { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string? Note { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }
    public DateOnly? ExpiredOn { get; private set; }

    public static Subscription StartAtListPrice(Guid subscriberId, Plan plan, DateTimeOffset createdOn)
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
        Guid subscriberId,
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
            Id = Guid.CreateVersion7(),
            SubscriberId = subscriberId,
            PlanCode = CatalogRules.RequireCode(planCode, nameof(planCode)),
            Price = CatalogRules.RequirePrice(price, nameof(price)),
            Currency = CatalogRules.RequireCurrency(currency, nameof(currency)),
            Note = trimmedNote,
            CreatedOn = createdOn.ToUniversalTime(),
        };
    }

    public void Expire(DateOnly expiredOn)
    {
        if (expiredOn < this.CreatedDate())
        {
            throw new ArgumentOutOfRangeException(nameof(expiredOn), "A subscription can't expire before it starts.");
        }

        ExpiredOn = expiredOn;
    }

    public void Delete(DateTimeOffset deletedOn) => DeletedOn = DeletedOnGuard.Delete(DeletedOn, deletedOn);
}
