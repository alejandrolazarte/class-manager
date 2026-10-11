using Microsoft.EntityFrameworkCore;

namespace ClassManager.Subscriptions.AspNetCore.Limits;

public sealed class FeatureLimits<TDbContext>
    where TDbContext : DbContext
{
    private const long SingleUnit = 1;

    private readonly List<FeatureLimitRegistration<TDbContext>> _registrations = [];

    internal IReadOnlyList<FeatureLimitRegistration<TDbContext>> Registrations => _registrations;

    public FeatureLimits<TDbContext> Count<TEntity>(
        string featureCode,
        Func<TDbContext, IServiceProvider, CancellationToken, Task<int>> countCurrent,
        Func<TEntity, bool>? countsWhenAdded = null)
        where TEntity : class
    {
        _registrations.Add(new FeatureLimitRegistration<TDbContext>(
            featureCode,
            entity => entity is TEntity typedEntity && (countsWhenAdded?.Invoke(typedEntity) ?? true) ? SingleUnit : 0,
            AmountWhenRemoved: null,
            async (context, services, cancellationToken) => await countCurrent(context, services, cancellationToken),
            SingleUnit));
        return this;
    }

    public FeatureLimits<TDbContext> Sum<TEntity>(
        string featureCode,
        Func<TDbContext, IServiceProvider, CancellationToken, Task<long>> sumCurrent,
        Func<TEntity, long> amountOf,
        long amountPerLimitUnit)
        where TEntity : class
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amountPerLimitUnit);
        Func<object, long> amountOfEntity = entity => entity is TEntity typedEntity ? amountOf(typedEntity) : 0;
        _registrations.Add(new FeatureLimitRegistration<TDbContext>(
            featureCode,
            amountOfEntity,
            amountOfEntity,
            sumCurrent,
            amountPerLimitUnit));
        return this;
    }
}

internal sealed record FeatureLimitRegistration<TDbContext>(
    string FeatureCode,
    Func<object, long> AmountWhenAdded,
    Func<object, long>? AmountWhenRemoved,
    Func<TDbContext, IServiceProvider, CancellationToken, Task<long>> MeasureCurrent,
    long AmountPerLimitUnit)
    where TDbContext : DbContext
{
    public async Task<int> UsageInLimitUnitsAsync(TDbContext context, IServiceProvider services, CancellationToken cancellationToken)
    {
        var amount = await MeasureCurrent(context, services, cancellationToken);
        return (int)((amount + AmountPerLimitUnit - 1) / AmountPerLimitUnit);
    }
}
