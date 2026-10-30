namespace ClassManager.Subscriptions.U.Tests;

internal static class BillingTestData
{
    public const string ExternalSubscriptionId = "sub_current";
    public const string OtherExternalSubscriptionId = "sub_other";
    public const string LitePlanCode = "lite";
    public const string ProPlanCode = "pro";
    public const string TrialPlanCode = "free";
    public const string Euro = "EUR";
    public const decimal LitePrice = 19m;
    public const decimal ProPrice = 39m;
    public const decimal OfferedLitePrice = 10m;

    public static readonly IReadOnlySet<string> PlanCodes = new HashSet<string> { TrialPlanCode, LitePlanCode, ProPlanCode };
    public static readonly DateTimeOffset Now = SubscriptionsTestData.Now;
    public static readonly DateOnly Today = SubscriptionsTestData.Today;

    public static ExternalSubscription External(
        string planCode = ProPlanCode,
        decimal unitAmount = ProPrice,
        ExternalSubscriptionStatus status = ExternalSubscriptionStatus.Active,
        string id = ExternalSubscriptionId,
        long? subscriberId = SubscriptionsTestData.SubscriberId,
        DateOnly? cancelOn = null,
        DateOnly? endedOn = null) =>
        new(id, subscriberId, planCode, unitAmount, Euro, status, cancelOn, endedOn);

    public static Subscription Trial() =>
        Subscription.Start(SubscriptionsTestData.SubscriberId, TrialPlanCode, price: 0m, Euro, note: null, Now.AddDays(-5));

    public static Subscription Paying(string planCode = ProPlanCode, decimal price = ProPrice, string id = ExternalSubscriptionId) =>
        Subscription.StartExternal(SubscriptionsTestData.SubscriberId, planCode, price, Euro, id, Now.AddDays(-5));

    public static PlanOffer LiteOffer() =>
        PlanOffer.Create(SubscriptionsTestData.SubscriberId, LitePlanCode, OfferedLitePrice, Euro, note: null, expiredOn: null, Now.AddDays(-1));

    public static ExternalSubscriptionSyncResult Sync(Subscription? current, ExternalSubscription external) =>
        ExternalSubscriptionSync.Apply(current, external, PlanCodes, Now);
}
