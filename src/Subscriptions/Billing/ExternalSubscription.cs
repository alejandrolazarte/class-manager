namespace ClassManager.Subscriptions.Billing;

public sealed record ExternalSubscription(
    string Id,
    long? SubscriberId,
    string? PriceLookupKey,
    decimal UnitAmount,
    string Currency,
    ExternalSubscriptionStatus Status,
    DateOnly? CancelOn,
    DateOnly? EndedOn);
