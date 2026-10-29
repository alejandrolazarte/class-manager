using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Subscriptions.Billing;

public sealed record ExternalSubscriptionSyncResult(ExternalSubscriptionSyncOutcome Outcome, Subscription? Started = null);
