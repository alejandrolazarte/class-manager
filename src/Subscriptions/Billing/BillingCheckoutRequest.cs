namespace ClassManager.Subscriptions.Billing;

public sealed record BillingCheckoutRequest(
    long SubscriberId,
    string ExternalCustomerId,
    string PriceLookupKey,
    DateTimeOffset? FirstChargeAt,
    Uri SuccessUrl,
    Uri CancelUrl);
