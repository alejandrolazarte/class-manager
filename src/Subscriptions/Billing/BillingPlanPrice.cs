namespace ClassManager.Subscriptions.Billing;

public sealed record BillingPlanPrice(string PlanCode, string ProductName, decimal UnitAmount, string Currency);
