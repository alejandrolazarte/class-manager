using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.AspNetCore.Access;
using ClassManager.Subscriptions.AspNetCore.Limits;
using ClassManager.Subscriptions.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClassManager.Subscriptions.AspNetCore.Hosting;

public static class SubscriptionsServiceCollectionExtensions
{
    public static IServiceCollection AddSubscriptions<TDbContext, TSubscriberResolver>(
        this IServiceCollection services,
        Action<FeatureLimits<TDbContext>>? configureLimits = null)
        where TDbContext : DbContext, ISubscriptionsDbContext
        where TSubscriberResolver : class, ISubscriberResolver
    {
        var limits = new FeatureLimits<TDbContext>();
        configureLimits?.Invoke(limits);
        services.AddSingleton(limits);
        services.AddScoped<FeatureLimitSaveChangesInterceptor<TDbContext>>();
        services.AddScoped<IFeatureUsage, FeatureUsage<TDbContext>>();
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<ISubscriptionsDbContext>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>());
        services.AddScoped<ISubscriberResolver, TSubscriberResolver>();
        services.AddScoped<IFeatureAccess, FeatureAccess>();

        return services;
    }
}
