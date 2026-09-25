using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Api.Endpoints;

internal static class BusinessEndpoints
{
    public static IEndpointRouteBuilder MapBusinessEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.Business, GetCurrentBusinessAsync);
        endpoints.MapPut(ApiRoutes.Business, UpdateBusinessSettingsAsync).RequireAuthorization(AuthorizationPolicies.OwnerOnly);

        return endpoints;
    }

    private static async Task<IResult> GetCurrentBusinessAsync(
        IUseCase<GetCurrentBusinessQuery, BusinessResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetCurrentBusinessQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> UpdateBusinessSettingsAsync(
        UpdateBusinessSettingsCommand command,
        IUseCase<UpdateBusinessSettingsCommand, UpdateBusinessSettingsResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }
}
