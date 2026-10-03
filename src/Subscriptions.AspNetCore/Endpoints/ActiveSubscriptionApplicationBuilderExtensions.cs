using Microsoft.AspNetCore.Builder;

namespace ClassManager.Subscriptions.AspNetCore.Endpoints;

public static class ActiveSubscriptionApplicationBuilderExtensions
{
    public static IApplicationBuilder UseActiveSubscriptionForChanges(this IApplicationBuilder app) =>
        app.UseMiddleware<ActiveSubscriptionMiddleware>();

    public static TBuilder AllowInactiveSubscription<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder =>
        builder.WithMetadata(new AllowInactiveSubscriptionMetadata());
}
