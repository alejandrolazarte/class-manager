namespace ClassManager.Subscriptions.Billing;

public sealed record BillingCustomerRequest(long SubscriberId, string Name, string Email, string PreferredLanguage);
