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
        var addedEntities = EntitiesIn(context, EntityState.Added);
        if (addedEntities.Count == 0)
        {
            return;
        }

        var removedEntities = EntitiesIn(context, EntityState.Deleted);
        EffectiveFeatures? features = null;
        foreach (var registration in limits.Registrations)
        {
            var addedAmount = addedEntities.Sum(registration.AmountWhenAdded);
            if (addedAmount == 0)
            {
                continue;
            }

            features ??= await services.GetRequiredService<IFeatureAccess>().GetCurrentAsync(cancellationToken);
            if (!features.IsActive || features.LimitOf(registration.FeatureCode) is not { } limit)
            {
                continue;
            }

            var removedAmount = registration.AmountWhenRemoved is { } amountWhenRemoved ? removedEntities.Sum(amountWhenRemoved) : 0;
            var currentAmount = await registration.MeasureCurrent(context, services, cancellationToken);
            if (currentAmount + addedAmount - removedAmount > limit * registration.AmountPerLimitUnit)
            {
                throw new FeatureLimitReachedException(registration.FeatureCode);
            }
        }
    }

    private static List<object> EntitiesIn(TDbContext context, EntityState state) =>
        context.ChangeTracker.Entries()
            .Where(entry => entry.State == state)
            .Select(entry => entry.Entity)
            .ToList();
}
