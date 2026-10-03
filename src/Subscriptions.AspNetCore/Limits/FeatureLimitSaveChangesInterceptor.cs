using ClassManager.Subscriptions.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Subscriptions.AspNetCore.Limits;

public sealed class FeatureLimitSaveChangesInterceptor<TDbContext>(
    IServiceProvider services,
    FeatureLimits<TDbContext> limits) : SaveChangesInterceptor
    where TDbContext : DbContext
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) =>
        SavingChangesAsync(eventData, result).AsTask().GetAwaiter().GetResult();

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        if (eventData.Context is TDbContext context)
        {
            await EnsureWithinLimitsAsync(context, cancellationToken);
        }

        return result;
    }

    private async Task EnsureWithinLimitsAsync(TDbContext context, CancellationToken cancellationToken)
    {
        var addedEntities = context.ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => entry.Entity)
            .ToList();
        if (addedEntities.Count == 0)
        {
            return;
        }

        EffectiveFeatures? features = null;
        foreach (var registration in limits.Registrations)
        {
            var addedCount = addedEntities.Count(registration.CountsWhenAdded);
            if (addedCount == 0)
            {
                continue;
            }

            features ??= await services.GetRequiredService<IFeatureAccess>().GetCurrentAsync(cancellationToken);
            if (!features.IsActive || features.LimitOf(registration.FeatureCode) is not { } limit)
            {
                continue;
            }

            var currentCount = await registration.CountCurrent(context, services, cancellationToken);
            if (currentCount + addedCount > limit)
            {
                throw new FeatureLimitReachedException(registration.FeatureCode);
            }
        }
    }
}
