namespace ClassManager.Core.UseCases.Subscriptions;

public static class BillingErrorCodes
{
    public const string PlanNotAvailable = "billing.plan_not_available";
    public const string PlanNotAvailableMessage = "This plan can't be bought online.";
    public const string AlreadySubscribed = "billing.already_subscribed";
    public const string AlreadySubscribedMessage = "The brand already pays online. Manage the plan from the billing portal.";
    public const string NotAvailable = "billing.not_available";
    public const string NotAvailableMessage = "Paying the plan online isn't available in the brand's country yet.";
    public const string NoCustomer = "billing.no_customer";
    public const string NoCustomerMessage = "The brand has never paid online.";
    public const string NoAccountMessage = "The current account was not found.";
}
