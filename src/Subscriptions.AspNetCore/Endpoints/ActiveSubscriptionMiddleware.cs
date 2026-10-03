using ClassManager.Subscriptions.Access;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassManager.Subscriptions.AspNetCore.Endpoints;

internal sealed class ActiveSubscriptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, IFeatureAccess featureAccess)
    {
        if (!RequiresActiveSubscription(httpContext)
            || (await featureAccess.GetCurrentAsync(httpContext.RequestAborted)).IsActive)
        {
            await next(httpContext);
            return;
        }

        await TypedResults.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Detail = SubscriptionErrorCodes.InactiveMessage,
            Extensions = { [FeatureEndpointConventionBuilderExtensions.ErrorCodeExtension] = SubscriptionErrorCodes.Inactive },
        }).ExecuteAsync(httpContext);
    }

    private static bool RequiresActiveSubscription(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        return endpoint is not null
            && httpContext.User.Identity?.IsAuthenticated == true
            && !HttpMethods.IsGet(httpContext.Request.Method)
            && !HttpMethods.IsHead(httpContext.Request.Method)
            && !HttpMethods.IsOptions(httpContext.Request.Method)
            && endpoint.Metadata.GetMetadata<IAllowAnonymous>() is null
            && endpoint.Metadata.GetMetadata<AllowInactiveSubscriptionMetadata>() is null;
    }
}
