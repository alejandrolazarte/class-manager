using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.TeamNotifications;

namespace ClassManager.Api.Endpoints;

internal static class TeamNotificationEndpoints
{
    public static IEndpointRouteBuilder MapTeamNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var team = endpoints.MapGroup(ApiRoutes.Team).RequireMember();
        team.MapGet(ApiRoutes.Notifications, GetNotificationsAsync);
        team.MapPut(ApiRoutes.Notifications + ApiRoutes.Seen, MarkNotificationsSeenAsync);
        team.MapGet(ApiRoutes.PushKey, GetPushKeyAsync);
        team.MapPut(ApiRoutes.PushSubscription, SavePushSubscriptionAsync);
        team.MapDelete(ApiRoutes.PushSubscription, RemovePushSubscriptionAsync);

        return endpoints;
    }

    private static async Task<IResult> GetNotificationsAsync(
        IUseCase<GetTeamNotificationsQuery, TeamNotificationsResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetTeamNotificationsQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> MarkNotificationsSeenAsync(
        IUseCase<MarkTeamNotificationsSeenCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new MarkTeamNotificationsSeenCommand(), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> GetPushKeyAsync(
        IUseCase<GetTeamPushKeyQuery, TeamPushKeyResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetTeamPushKeyQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SavePushSubscriptionAsync(
        SaveTeamPushSubscriptionCommand command,
        IUseCase<SaveTeamPushSubscriptionCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> RemovePushSubscriptionAsync(
        string? endpoint,
        IUseCase<RemoveTeamPushSubscriptionCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoveTeamPushSubscriptionCommand(endpoint), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
