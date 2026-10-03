using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Subscriptions.Subscribers;

public sealed class Subscription
{
    private Subscription()
    {
    }

    public Guid Id { get; private set; }
    public Guid SubscriberId { get; private set; }
    public string PlanCode { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public DateOnly StartsOn { get; private set; }
    public DateOnly? EndsOn { get; private set; }
    public string? Note { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Subscription StartAtListPrice(Guid subscriberId, Plan plan, DateOnly startsOn, DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return Start(subscriberId, plan.Code, plan.ListPrice ?? 0m, plan.Currency, startsOn, note: null, createdAt);
    }

    public static Subscription Start(
        Guid subscriberId,
        string planCode,
        decimal price,
        string currency,
        DateOnly startsOn,
        string? note,
        DateTimeOffset createdAt)
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
            StartsOn = startsOn,
            Note = trimmedNote,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public void End(DateOnly endsOn)
    {
        if (endsOn < StartsOn)
        {
            throw new ArgumentOutOfRangeException(nameof(endsOn), "A subscription can't end before it starts.");
        }

        EndsOn = endsOn;
    }

    public bool IsActiveOn(DateOnly date) => StartsOn <= date && (EndsOn is null || date <= EndsOn);
}
