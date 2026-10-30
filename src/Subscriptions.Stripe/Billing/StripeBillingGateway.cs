using System.Globalization;
using ClassManager.Subscriptions.Billing;
using Stripe;
using Stripe.Checkout;

namespace ClassManager.Subscriptions.Stripe.Billing;

internal sealed class StripeBillingGateway(StripeClient client, TimeProvider timeProvider) : IBillingGateway
{
    private const string ResourceMissingCode = "resource_missing";
    private const decimal MinorUnitsPerMajorUnit = 100m;
    private const string SubscriptionMode = "subscription";
    private const string RequiredBillingAddress = "required";
    private const string UpdateFromCheckout = "auto";
    private const string LocaleFromBrowser = "auto";
    private const int SingleUnit = 1;
    private const string PriceNotFoundMessage = "No active Stripe price has the lookup key ";

    private static readonly TimeSpan ShortestTrial = TimeSpan.FromHours(48);

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

    public async Task<string> CreateCustomerAsync(BillingCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await client.V1.Customers.CreateAsync(
            new CustomerCreateOptions
            {
                Name = request.Name,
                Email = request.Email,
                PreferredLocales = [request.PreferredLanguage],
                Metadata = SubscriberMetadata(request.SubscriberId),
            },
            cancellationToken: cancellationToken);
        return customer.Id;
    }

    public async Task<Uri> CreateCheckoutAsync(BillingCheckoutRequest request, CancellationToken cancellationToken)
    {
        var priceId = await FindPriceIdAsync(request.PriceLookupKey, cancellationToken);
        var session = await client.V1.Checkout.Sessions.CreateAsync(
            new SessionCreateOptions
            {
                Mode = SubscriptionMode,
                Customer = request.ExternalCustomerId,
                ClientReferenceId = request.SubscriberId.ToString(CultureInfo.InvariantCulture),
                LineItems = [new SessionLineItemOptions { Price = priceId, Quantity = SingleUnit }],
                SubscriptionData = new SessionSubscriptionDataOptions
                {
                    Metadata = SubscriberMetadata(request.SubscriberId),
                    TrialEnd = TrialEndOrNull(request.FirstChargeAt),
                },
                AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
                TaxIdCollection = new SessionTaxIdCollectionOptions { Enabled = true },
                BillingAddressCollection = RequiredBillingAddress,
                CustomerUpdate = new SessionCustomerUpdateOptions { Address = UpdateFromCheckout, Name = UpdateFromCheckout },
                Locale = LocaleFromBrowser,
                SuccessUrl = request.SuccessUrl.ToString(),
                CancelUrl = request.CancelUrl.ToString(),
            },
            cancellationToken: cancellationToken);
        return new Uri(session.Url);
    }

    public async Task<Uri> CreatePortalSessionAsync(string externalCustomerId, Uri returnUrl, CancellationToken cancellationToken)
    {
        var session = await client.V1.BillingPortal.Sessions.CreateAsync(
            new global::Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = externalCustomerId,
                ReturnUrl = returnUrl.ToString(),
            },
            cancellationToken: cancellationToken);
        return new Uri(session.Url);
    }

    private async Task<string> FindPriceIdAsync(string lookupKey, CancellationToken cancellationToken)
    {
        var prices = await client.V1.Prices.ListAsync(
            new PriceListOptions { LookupKeys = [lookupKey], Active = true },
            cancellationToken: cancellationToken);
        return prices.Data.SingleOrDefault()?.Id ?? throw new InvalidOperationException(PriceNotFoundMessage + lookupKey);
    }

    private DateTime? TrialEndOrNull(DateTimeOffset? firstChargeAt) =>
        firstChargeAt is { } chargeAt && chargeAt - timeProvider.GetUtcNow() >= ShortestTrial
            ? chargeAt.UtcDateTime
            : null;

    private static Dictionary<string, string> SubscriberMetadata(long subscriberId) =>
        new(StringComparer.Ordinal) { [StripeMetadataKeys.SubscriberId] = subscriberId.ToString(CultureInfo.InvariantCulture) };

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
