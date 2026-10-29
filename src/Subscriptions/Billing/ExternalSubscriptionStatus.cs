namespace ClassManager.Subscriptions.Billing;

public enum ExternalSubscriptionStatus
{
    Incomplete,
    IncompleteExpired,
    Trialing,
    Active,
    PastDue,
    Unpaid,
    Paused,
    Canceled,
}
