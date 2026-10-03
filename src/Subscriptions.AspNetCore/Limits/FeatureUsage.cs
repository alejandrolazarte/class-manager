using ClassManager.Subscriptions.Access;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Subscriptions.AspNetCore.Limits;

internal sealed class FeatureUsage<TDbContext>(
    TDbContext context,
    IServiceProvider services,
    FeatureLimits<TDbContext> limits) : IFeatureUsage
    where TDbContext : DbContext
{
    public IReadOnlyCollection<string> CountedFeatureCodes =>
        [.. limits.Registrations.Select(registration => registration.FeatureCode).Distinct(StringComparer.Ordinal)];

    public Task<int> CountAsync(string featureCode, CancellationToken cancellationToken)
    {
        var registration = limits.Registrations.FirstOrDefault(candidate => candidate.FeatureCode == featureCode)
            ?? throw new ArgumentException($"No limit is registered for '{featureCode}'.", nameof(featureCode));
        return registration.CountCurrent(context, services, cancellationToken);
    }
}
