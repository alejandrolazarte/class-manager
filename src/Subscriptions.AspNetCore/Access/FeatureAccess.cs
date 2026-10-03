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

        var subscription = await context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.SubscriberId == subscriberId && candidate.DeletedOn == null, cancellationToken);
        if (subscription is null || !subscription.IsActiveOn(today))
        {
            return EffectiveFeatures.Inactive(subscription?.PlanCode ?? string.Empty, subscription?.ExpiredOn);
        }

        var planFeatures = await context.PlanFeatures
            .AsNoTracking()
            .Where(planFeature => planFeature.PlanCode == subscription.PlanCode)
            .ToListAsync(cancellationToken);
        var subscriptionFeatures = await context.SubscriptionFeatures
            .AsNoTracking()
            .Where(subscriptionFeature => subscriptionFeature.SubscriptionId == subscription.Id && subscriptionFeature.DeletedOn == null)
            .ToListAsync(cancellationToken);

        return EffectiveFeatures.Combine(subscription.PlanCode, planFeatures, subscriptionFeatures, today, subscription.ExpiredOn);
    }
}
