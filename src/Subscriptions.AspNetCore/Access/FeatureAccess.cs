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
        var subscriberId = await subscriberResolver.ResolveCurrentSubscriberIdAsync(cancellationToken);
        var subscription = subscriberId is { } id ? await ActiveSubscriptionAsync(id, today, cancellationToken) : null;
        var planCode = subscription?.PlanCode ?? await DefaultPlanCodeAsync(cancellationToken);
        if (planCode is null)
        {
            return EffectiveFeatures.Combine(string.Empty, [], [], today);
        }

        var planFeatures = await context.PlanFeatures
            .AsNoTracking()
            .Where(planFeature => planFeature.PlanCode == planCode)
            .ToListAsync(cancellationToken);
        var subscriptionFeatures = subscription is null
            ? []
            : await context.SubscriptionFeatures
                .AsNoTracking()
                .Where(subscriptionFeature => subscriptionFeature.SubscriptionId == subscription.Id)
                .ToListAsync(cancellationToken);

        return EffectiveFeatures.Combine(planCode, planFeatures, subscriptionFeatures, today);
    }

    private Task<Subscription?> ActiveSubscriptionAsync(Guid subscriberId, DateOnly today, CancellationToken cancellationToken) =>
        context.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.SubscriberId == subscriberId
                && subscription.StartsOn <= today
                && (subscription.EndsOn == null || subscription.EndsOn >= today))
            .OrderByDescending(subscription => subscription.StartsOn)
            .FirstOrDefaultAsync(cancellationToken);

    private Task<string?> DefaultPlanCodeAsync(CancellationToken cancellationToken) =>
        context.Plans
            .AsNoTracking()
            .Where(plan => plan.IsDefault)
            .Select(plan => plan.Code)
            .FirstOrDefaultAsync(cancellationToken);
}
