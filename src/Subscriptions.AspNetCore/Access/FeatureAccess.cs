using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.AspNetCore.Persistence;
using ClassManager.Subscriptions.Subscribers;
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

        if (await ActiveSubscriptionAsync(subscriberId, today, cancellationToken) is not { } subscription)
        {
            return EffectiveFeatures.Inactive(await LastPlanCodeAsync(subscriberId, cancellationToken) ?? string.Empty);
        }

        var planFeatures = await context.PlanFeatures
            .AsNoTracking()
            .Where(planFeature => planFeature.PlanCode == subscription.PlanCode)
            .ToListAsync(cancellationToken);
        var subscriptionFeatures = await context.SubscriptionFeatures
            .AsNoTracking()
            .Where(subscriptionFeature => subscriptionFeature.SubscriptionId == subscription.Id)
            .ToListAsync(cancellationToken);

        return EffectiveFeatures.Combine(subscription.PlanCode, planFeatures, subscriptionFeatures, today);
    }

    private Task<Subscription?> ActiveSubscriptionAsync(Guid subscriberId, DateOnly today, CancellationToken cancellationToken) =>
        context.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.SubscriberId == subscriberId
                && subscription.StartsOn <= today
                && (subscription.EndsOn == null || subscription.EndsOn >= today))
            .OrderByDescending(subscription => subscription.StartsOn)
            .ThenByDescending(subscription => subscription.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    private Task<string?> LastPlanCodeAsync(Guid subscriberId, CancellationToken cancellationToken) =>
        context.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.SubscriberId == subscriberId)
            .OrderByDescending(subscription => subscription.StartsOn)
            .ThenByDescending(subscription => subscription.CreatedAt)
            .Select(subscription => subscription.PlanCode)
            .FirstOrDefaultAsync(cancellationToken);
}
