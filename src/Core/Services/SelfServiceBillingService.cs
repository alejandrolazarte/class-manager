using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Subscriptions;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Records;
using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.Offers;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Core.Services;

public sealed class SelfServiceBillingService(
    IBusinessRepository businessRepository,
    ISubscriptionRepository subscriptionRepository,
    TimeProvider timeProvider) : ISelfServiceBillingService
{
    public async Task<SelfServiceBillingAccess> GetAccessAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        var today = Today();
        var planFeatures = (await subscriptionRepository.ListPlanFeaturesAsync(cancellationToken))
            .Where(planFeature => planFeature.PlanCode == subscription.PlanCode);
        var subscriptionFeatures = await subscriptionRepository.ListFeaturesAsync(subscription.Id, cancellationToken);
        var features = EffectiveFeatures.Combine(subscription.PlanCode, planFeatures, subscriptionFeatures, today);
        if (!features.Has(Features.SelfServiceBilling))
        {
            return SelfServiceBillingAccess.NotInPlan;
        }

        return await IsInBillingCountryAsync(subscription.SubscriberId, cancellationToken)
            ? SelfServiceBillingAccess.Available
            : SelfServiceBillingAccess.CountryNotSupported;
    }

    public async Task<PlanOffer?> FindActiveOfferAsync(long subscriberId, CancellationToken cancellationToken)
    {
        var offer = await subscriptionRepository.GetCurrentOfferAsync(subscriberId, cancellationToken);
        return offer is not null && offer.IsActiveOn(Today()) ? offer : null;
    }

    public async Task<SelfServiceBillingAccess> GetAccessAsync(PlanOffer offer, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(offer);
        return await IsInBillingCountryAsync(offer.SubscriberId, cancellationToken)
            ? SelfServiceBillingAccess.Available
            : SelfServiceBillingAccess.CountryNotSupported;
    }

    private async Task<bool> IsInBillingCountryAsync(long subscriberId, CancellationToken cancellationToken)
    {
        var countryCode = await businessRepository.FindFirstBranchCountryCodeAsync(subscriberId, cancellationToken);
        return countryCode is not null && SelfServiceBilling.CountryCodes.Contains(countryCode);
    }

    private DateOnly Today() => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
}
