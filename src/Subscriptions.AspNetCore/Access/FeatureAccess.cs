using ClassManager.Records;
using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Subscriptions.AspNetCore.Access;

internal sealed class FeatureAccess(
    ISubscriptionsDbContext context,
    ISubscriberResolver subscriberResolver,
    TimeProvider timeProvider) : IFeatureAccess
{
    private EffectiveFeatures? _resolvedFeatures;

    public async Task<EffectiveFeatures> GetCurrentAsync(CancellationToken cancellationToken) =>
        _resolvedFeatures ??= await ResolveAsync(cancellationToken);

    private async Task<EffectiveFeatures> ResolveAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (await subscriberResolver.ResolveCurrentSubscriberIdAsync(cancellationToken) is not { } subscriberId)
        {
            return EffectiveFeatures.Inactive(string.Empty);
        }

        var planFeatures = context.PlanFeatures.AsNoTracking();
        var subscriptionFeatures = context.SubscriptionFeatures.AsNoTracking().WhereNotDeleted();
        var current = await context.Subscriptions
            .AsNoTracking()
            .WhereNotDeleted()
            .Where(candidate => candidate.SubscriberId == subscriberId)
            .Select(subscription => new
            {
                Subscription = subscription,
                PlanFeatures = planFeatures.Where(planFeature => planFeature.PlanCode == subscription.PlanCode).ToList(),
                SubscriptionFeatures = subscriptionFeatures.Where(subscriptionFeature => subscriptionFeature.SubscriptionId == subscription.Id).ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (current is null || !current.Subscription.IsActiveOn(today))
        {
            return EffectiveFeatures.Inactive(current?.Subscription.PlanCode ?? string.Empty, current?.Subscription.ExpiredOn);
        }

        return EffectiveFeatures.Combine(
            current.Subscription.PlanCode, current.PlanFeatures, current.SubscriptionFeatures, today, current.Subscription.ExpiredOn);
    }
}
