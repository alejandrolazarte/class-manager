namespace ClassManager.Subscriptions.Billing;

public sealed record BillingCheckoutRequest(
    long SubscriberId,
    string ExternalCustomerId,
    BillingPlanPrice Plan,
    DateTimeOffset? FirstChargeAt,
    Uri SuccessUrl,
    Uri CancelUrl);
