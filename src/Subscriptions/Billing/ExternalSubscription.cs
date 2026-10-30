namespace ClassManager.Subscriptions.Billing;

public sealed record ExternalSubscription(
    string Id,
    long? SubscriberId,
    string? PlanCode,
    decimal UnitAmount,
    string Currency,
    ExternalSubscriptionStatus Status,
    DateOnly? CancelOn,
    DateOnly? EndedOn);
