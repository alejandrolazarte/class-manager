using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Subscriptions;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;
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

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var planFeatures = (await subscriptionRepository.ListPlanFeaturesAsync(cancellationToken))
            .Where(planFeature => planFeature.PlanCode == subscription.PlanCode);
        var subscriptionFeatures = await subscriptionRepository.ListFeaturesAsync(subscription.Id, cancellationToken);
        var features = EffectiveFeatures.Combine(subscription.PlanCode, planFeatures, subscriptionFeatures, today);
        if (!features.Has(Features.SelfServiceBilling))
        {
            return SelfServiceBillingAccess.NotInPlan;
        }

        var countryCode = await businessRepository.FindFirstBranchCountryCodeAsync(subscription.SubscriberId, cancellationToken);
        return countryCode is not null && SelfServiceBilling.CountryCodes.Contains(countryCode)
            ? SelfServiceBillingAccess.Available
            : SelfServiceBillingAccess.CountryNotSupported;
    }
}
