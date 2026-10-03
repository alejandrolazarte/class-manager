using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.AspNetCore.Endpoints;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassManager.Subscriptions.AspNetCore.Limits;

public sealed class FeatureLimitExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        if (exception is not FeatureLimitReachedException limitReached)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Detail = FeatureErrorCodes.LimitReachedMessage,
                Extensions =
                {
                    [FeatureEndpointConventionBuilderExtensions.ErrorCodeExtension] = FeatureErrorCodes.LimitReached,
                    [FeatureErrorCodes.FeatureDetail] = limitReached.FeatureCode,
                },
            },
            Exception = exception,
        });
    }
}
