using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Subscriptions.Subscribers;

public static class SalesFees
{
    public static decimal PercentOf(Plan? plan, SalesFeeOverride? subscriberFee) =>
        subscriberFee?.Percent ?? plan?.SalesFeePercent ?? 0m;
}
