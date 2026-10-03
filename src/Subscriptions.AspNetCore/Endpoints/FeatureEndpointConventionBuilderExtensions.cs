using ClassManager.Subscriptions.Access;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Subscriptions.AspNetCore.Endpoints;

public static class FeatureEndpointConventionBuilderExtensions
{
    public const string ErrorCodeExtension = "code";

    public static TBuilder RequireFeature<TBuilder>(this TBuilder builder, string featureCode)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.WithMetadata(new RequiredFeatureMetadata(featureCode));
        builder.AddEndpointFilter(async (invocationContext, next) =>
        {
            var featureAccess = invocationContext.HttpContext.RequestServices.GetRequiredService<IFeatureAccess>();
            var features = await featureAccess.GetCurrentAsync(invocationContext.HttpContext.RequestAborted);
            return features.Has(featureCode) ? await next(invocationContext) : NotInPlan(featureCode);
        });

        return builder;
    }

    private static ProblemHttpResult NotInPlan(string featureCode) =>
        TypedResults.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Detail = FeatureErrorCodes.NotInPlanMessage,
            Extensions =
            {
                [ErrorCodeExtension] = FeatureErrorCodes.NotInPlan,
                [FeatureErrorCodes.FeatureDetail] = featureCode,
            },
        });
}
