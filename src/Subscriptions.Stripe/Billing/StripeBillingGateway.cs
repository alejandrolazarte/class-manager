using System.Globalization;
using ClassManager.Subscriptions.Billing;
using Stripe;

namespace ClassManager.Subscriptions.Stripe.Billing;

internal sealed class StripeBillingGateway(StripeClient client) : IBillingGateway
{
    private const string ResourceMissingCode = "resource_missing";
    private const decimal MinorUnitsPerMajorUnit = 100m;

    private static readonly Dictionary<string, ExternalSubscriptionStatus> Statuses = new(StringComparer.Ordinal)
    {
        ["incomplete"] = ExternalSubscriptionStatus.Incomplete,
        ["incomplete_expired"] = ExternalSubscriptionStatus.IncompleteExpired,
        ["trialing"] = ExternalSubscriptionStatus.Trialing,
        ["active"] = ExternalSubscriptionStatus.Active,
        ["past_due"] = ExternalSubscriptionStatus.PastDue,
        ["unpaid"] = ExternalSubscriptionStatus.Unpaid,
        ["paused"] = ExternalSubscriptionStatus.Paused,
        ["canceled"] = ExternalSubscriptionStatus.Canceled,
    };

    public async Task<ExternalSubscription?> GetSubscriptionAsync(string externalSubscriptionId, CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await client.V1.Subscriptions.GetAsync(externalSubscriptionId, cancellationToken: cancellationToken);
            return ToExternal(subscription);
        }
        catch (StripeException exception) when (exception.StripeError?.Code == ResourceMissingCode)
        {
            return null;
        }
    }

    private static ExternalSubscription ToExternal(Subscription subscription)
    {
        var item = subscription.Items.Data.Single();
        var cancelAt = subscription.CancelAt ?? (subscription.CancelAtPeriodEnd ? item.CurrentPeriodEnd : null);

        return new ExternalSubscription(
            subscription.Id,
            SubscriberIdOf(subscription),
            item.Price.LookupKey,
            (item.Price.UnitAmount ?? 0) / MinorUnitsPerMajorUnit,
            item.Price.Currency.ToUpperInvariant(),
            Statuses[subscription.Status],
            DateOfOrNull(cancelAt),
            DateOfOrNull(subscription.EndedAt));
    }

    private static long? SubscriberIdOf(Subscription subscription) =>
        subscription.Metadata.TryGetValue(StripeMetadataKeys.SubscriberId, out var value)
        && long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var subscriberId)
            ? subscriberId
            : null;

    private static DateOnly? DateOfOrNull(DateTime? moment) => moment is { } value ? DateOnly.FromDateTime(value) : null;
}
