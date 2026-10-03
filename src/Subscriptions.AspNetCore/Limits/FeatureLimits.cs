using Microsoft.EntityFrameworkCore;

namespace ClassManager.Subscriptions.AspNetCore.Limits;

public sealed class FeatureLimits<TDbContext>
    where TDbContext : DbContext
{
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
            entity => entity is TEntity typedEntity && (countsWhenAdded?.Invoke(typedEntity) ?? true),
            countCurrent));
        return this;
    }
}

internal sealed record FeatureLimitRegistration<TDbContext>(
    string FeatureCode,
    Func<object, bool> CountsWhenAdded,
    Func<TDbContext, IServiceProvider, CancellationToken, Task<int>> CountCurrent)
    where TDbContext : DbContext;
