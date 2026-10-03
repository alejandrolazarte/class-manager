using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.AspNetCore.Access;
using ClassManager.Subscriptions.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClassManager.Subscriptions.AspNetCore.Hosting;

public static class SubscriptionsServiceCollectionExtensions
{
    public static IServiceCollection AddSubscriptions<TDbContext, TSubscriberResolver>(this IServiceCollection services)
        where TDbContext : DbContext, ISubscriptionsDbContext
        where TSubscriberResolver : class, ISubscriberResolver
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<ISubscriptionsDbContext>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>());
        services.AddScoped<ISubscriberResolver, TSubscriberResolver>();
        services.AddScoped<IFeatureAccess, FeatureAccess>();

        return services;
    }
}
