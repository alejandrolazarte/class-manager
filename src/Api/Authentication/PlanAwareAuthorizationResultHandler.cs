using ClassManager.Api.ErrorHandling;
using ClassManager.Subscriptions.Access;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;

namespace ClassManager.Api.Authentication;

internal sealed class PlanAwareAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        var failureReasons = authorizeResult.AuthorizationFailure?.FailureReasons ?? [];
        var problem = failureReasons.Select(ProblemFor).FirstOrDefault(candidate => candidate is not null);
        if (!authorizeResult.Forbidden || problem is null)
        {
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }

        await TypedResults.Problem(problem).ExecuteAsync(context);
    }

    private static ProblemDetails? ProblemFor(AuthorizationFailureReason reason) => reason switch
    {
        FeatureNotInPlanReason featureNotInPlan => new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Detail = FeatureErrorCodes.NotInPlanMessage,
            Extensions =
            {
                [ResultHttpExtensions.ErrorCodeExtension] = FeatureErrorCodes.NotInPlan,
                [FeatureErrorCodes.FeatureDetail] = featureNotInPlan.FeatureCode,
            },
        },
        SubscriptionInactiveReason => new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Detail = SubscriptionErrorCodes.InactiveMessage,
            Extensions = { [ResultHttpExtensions.ErrorCodeExtension] = SubscriptionErrorCodes.Inactive },
        },
        _ => null,
    };
}
