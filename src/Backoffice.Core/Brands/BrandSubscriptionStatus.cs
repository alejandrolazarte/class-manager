using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Records;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.Brands;

public static class BrandSubscriptionStatus
{
    public static BrandStatus Of(Subscription? currentSubscription, DateOnly today) =>
        currentSubscription switch
        {
            null => BrandStatus.WithoutSubscription,
            _ when !currentSubscription.IsActiveOn(today) => BrandStatus.Ended,
            { PlanCode: PlanCodes.Free } => BrandStatus.Trial,
            _ => BrandStatus.Active,
        };

    public static int? TrialDaysLeft(Subscription? currentSubscription, DateOnly today) =>
        Of(currentSubscription, today) == BrandStatus.Trial && currentSubscription!.ExpiredOn is { } expiredOn
            ? expiredOn.DayNumber - today.DayNumber + 1
            : null;
}
