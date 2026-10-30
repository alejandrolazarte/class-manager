namespace ClassManager.Subscriptions.Billing;

public enum ExternalSubscriptionSyncOutcome
{
    Applied,
    MissingSubscriber,
    NotStarted,
    NotCurrent,
    UnknownPlan,
}
