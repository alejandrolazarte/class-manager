using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Achievements;
using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Api.Endpoints;

internal static class BusinessEndpoints
{
    public static IEndpointRouteBuilder MapBusinessEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.Business, GetCurrentBusinessAsync).RequirePermission(Permissions.Business.View);
        endpoints.MapPut(ApiRoutes.Business, UpdateBusinessSettingsAsync).RequirePermission(Permissions.Business.Manage);
        endpoints.MapGet(ApiRoutes.Business + ApiRoutes.Achievements, GetAchievementSettingsAsync).RequirePermission(Permissions.Business.View);
        endpoints.MapPut(ApiRoutes.Business + ApiRoutes.Achievements, UpdateAchievementSettingsAsync).RequirePermission(Permissions.Business.Manage);

        return endpoints;
    }

    private static async Task<IResult> GetCurrentBusinessAsync(
        IUseCase<GetCurrentBusinessQuery, BusinessResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetCurrentBusinessQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetAchievementSettingsAsync(
        IUseCase<GetAchievementSettingsQuery, AchievementSettingsResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetAchievementSettingsQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> UpdateAchievementSettingsAsync(
        UpdateAchievementSettingsCommand command,
        IUseCase<UpdateAchievementSettingsCommand, AchievementSettingsResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

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
