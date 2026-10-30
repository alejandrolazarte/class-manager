namespace ClassManager.Subscriptions.Billing;

public sealed record BillingPortalRequest(string ExternalCustomerId, IReadOnlyList<BillingPlanPrice> SwitchablePlans, Uri ReturnUrl);
