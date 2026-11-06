using System.Security.Claims;
using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Notifications;
using ClassManager.Security.Hosting;

namespace ClassManager.Api.Endpoints;

internal static class NotificationPreferenceEndpoints
{
    public static IEndpointRouteBuilder MapNotificationPreferenceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var preferences = endpoints.MapGroup(ApiRoutes.MyNotificationPreferences).RequireAnyAccount();
        preferences.MapGet(string.Empty, ListMyNotificationPreferencesAsync);
        preferences.MapPut(string.Empty, UpdateMyNotificationPreferenceAsync);

        var unsubscribe = endpoints.MapGroup(ApiRoutes.EmailUnsubscribe)
            .AllowAnonymous()
            .RequireRateLimiting(SecurityServiceCollectionExtensions.AuthenticationRateLimitPolicy);
        unsubscribe.MapGet(string.Empty, GetEmailUnsubscriptionAsync);
        unsubscribe.MapPost(string.Empty, UnsubscribeFromEmailAsync).DisableAntiforgery();
        unsubscribe.MapPost(ApiRoutes.EmailResubscribe, ResubscribeToEmailAsync);

        return endpoints;
    }

    private static async Task<IResult> ListMyNotificationPreferencesAsync(
        ClaimsPrincipal user,
        IUseCase<ListMyNotificationPreferencesQuery, NotificationPreferencesResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListMyNotificationPreferencesQuery(SessionKindClaim.Of(user)), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetEmailUnsubscriptionAsync(
        string? token,
        IUseCase<GetEmailUnsubscriptionQuery, EmailUnsubscriptionResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetEmailUnsubscriptionQuery(token), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> UnsubscribeFromEmailAsync(
        string? token,
        IUseCase<UnsubscribeFromEmailCommand, EmailUnsubscriptionResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new UnsubscribeFromEmailCommand(token), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ResubscribeToEmailAsync(
        string? token,
        IUseCase<ResubscribeToEmailCommand, EmailUnsubscriptionResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ResubscribeToEmailCommand(token), cancellationToken);

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
