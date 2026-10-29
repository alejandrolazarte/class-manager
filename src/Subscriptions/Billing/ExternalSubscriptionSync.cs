using ClassManager.Records;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Subscriptions.Billing;

public static class ExternalSubscriptionSync
{
    public static ExternalSubscriptionSyncResult Apply(
        Subscription? current,
        ExternalSubscription external,
        IReadOnlySet<string> planCodes,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(external);
        ArgumentNullException.ThrowIfNull(planCodes);

        if (external.SubscriberId is not { } subscriberId)
        {
            return new ExternalSubscriptionSyncResult(ExternalSubscriptionSyncOutcome.MissingSubscriber);
        }

        if (external.Status is ExternalSubscriptionStatus.Incomplete or ExternalSubscriptionStatus.IncompleteExpired)
        {
            return new ExternalSubscriptionSyncResult(ExternalSubscriptionSyncOutcome.NotStarted);
        }

        var isCurrent = current is not null && current.ExternalSubscriptionId == external.Id;
        if (external.Status is ExternalSubscriptionStatus.Canceled or ExternalSubscriptionStatus.Paused)
        {
            if (!isCurrent)
            {
                return new ExternalSubscriptionSyncResult(ExternalSubscriptionSyncOutcome.NotCurrent);
            }

            End(current!, external.EndedOn ?? DateOnly.FromDateTime(now.UtcDateTime));
            return new ExternalSubscriptionSyncResult(ExternalSubscriptionSyncOutcome.Applied);
        }

        var planCode = PlanPriceLookupKey.PlanCodeOf(external.PriceLookupKey);
        if (planCode is null || !planCodes.Contains(planCode))
        {
            return new ExternalSubscriptionSyncResult(ExternalSubscriptionSyncOutcome.UnknownPrice);
        }

        Subscription? started = null;
        if (!isCurrent || !HasSameTerms(current!, planCode, external))
        {
            current?.Delete(now);
            started = Subscription.StartExternal(subscriberId, planCode, external.UnitAmount, external.Currency, external.Id, now);
        }

        var subscription = started ?? current!;
        ApplyCancellation(subscription, external.CancelOn);
        subscription.ChangePaymentStatus(PaymentStatusOf(external.Status));
        return new ExternalSubscriptionSyncResult(ExternalSubscriptionSyncOutcome.Applied, started);
    }

    private static bool HasSameTerms(Subscription subscription, string planCode, ExternalSubscription external) =>
        subscription.PlanCode == planCode
        && subscription.Price == external.UnitAmount
        && string.Equals(subscription.Currency, external.Currency, StringComparison.OrdinalIgnoreCase);

    private static void End(Subscription subscription, DateOnly endedOn)
    {
        if (subscription.ExpiredOn is null || subscription.ExpiredOn > endedOn)
        {
            subscription.Expire(NotBeforeStart(subscription, endedOn));
        }
    }

    private static void ApplyCancellation(Subscription subscription, DateOnly? cancelOn)
    {
        if (cancelOn is { } lastDay)
        {
            subscription.Expire(NotBeforeStart(subscription, lastDay));
            return;
        }

        subscription.ClearExpiry();
    }

    private static DateOnly NotBeforeStart(Subscription subscription, DateOnly date)
    {
        var createdDate = subscription.CreatedDate();
        return date < createdDate ? createdDate : date;
    }

    private static SubscriptionPaymentStatus PaymentStatusOf(ExternalSubscriptionStatus status) =>
        status is ExternalSubscriptionStatus.PastDue or ExternalSubscriptionStatus.Unpaid
            ? SubscriptionPaymentStatus.PastDue
            : SubscriptionPaymentStatus.Current;
}
