using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Api.Endpoints;

internal static class NotificationPreferenceEndpoints
{
    public static IEndpointRouteBuilder MapNotificationPreferenceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var preferences = endpoints.MapGroup(ApiRoutes.MyNotificationPreferences).RequireAnyAccount();
        preferences.MapGet(string.Empty, ListMyNotificationPreferencesAsync);
        preferences.MapPut(string.Empty, UpdateMyNotificationPreferenceAsync);

        return endpoints;
    }

    private static async Task<IResult> ListMyNotificationPreferencesAsync(
        IUseCase<ListMyNotificationPreferencesQuery, IReadOnlyList<NotificationPreferenceResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListMyNotificationPreferencesQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> UpdateMyNotificationPreferenceAsync(
        UpdateMyNotificationPreferenceCommand command,
        IUseCase<UpdateMyNotificationPreferenceCommand, NotificationPreferenceResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }
}
