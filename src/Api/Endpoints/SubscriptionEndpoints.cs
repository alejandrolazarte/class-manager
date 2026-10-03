using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Subscriptions;

namespace ClassManager.Api.Endpoints;

internal static class SubscriptionEndpoints
{
    public static IEndpointRouteBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.Plans, ListPlansAsync).AllowAnonymous();
        endpoints.MapGet(ApiRoutes.OrganizationSubscription, GetOrganizationSubscriptionAsync)
            .RequirePermission(Permissions.Brand.ViewSubscription);

        return endpoints;
    }

    private static async Task<IResult> ListPlansAsync(
        IUseCase<ListPlansQuery, IReadOnlyList<PlanResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListPlansQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetOrganizationSubscriptionAsync(
        IUseCase<GetOrganizationSubscriptionQuery, OrganizationSubscriptionResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetOrganizationSubscriptionQuery(), cancellationToken);

        return result.ToOkResult();
    }
}
