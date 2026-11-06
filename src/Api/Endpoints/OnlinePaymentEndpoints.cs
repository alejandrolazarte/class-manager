using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Core.UseCases.OnlinePayments;

namespace ClassManager.Api.Endpoints;

internal static class OnlinePaymentEndpoints
{
    public const string StripeSignatureHeaderName = "Stripe-Signature";

    public static IEndpointRouteBuilder MapOnlinePaymentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.OnlinePayments, GetOnlinePaymentsAsync)
            .RequirePermission(Permissions.OnlinePayments.Manage);
        endpoints.MapPost(ApiRoutes.OnlinePaymentsOnboarding, StartOnboardingAsync)
            .RequirePermission(Permissions.OnlinePayments.Manage);
        endpoints.MapPost(ApiRoutes.OnlinePaymentsRefresh, RefreshAsync)
            .RequirePermission(Permissions.OnlinePayments.Manage);
        endpoints.MapPut(ApiRoutes.OnlinePaymentsAccount, UseAccountAsync)
            .RequirePermission(Permissions.OnlinePayments.Share);
        endpoints.MapDelete(ApiRoutes.OnlinePaymentsAccount, StopUsingAccountAsync)
            .RequirePermission(Permissions.OnlinePayments.Manage);
        endpoints.MapPost(ApiRoutes.StripeConnectWebhook, ReceiveStripeEventAsync)
            .AllowAnonymous()
            .AllowInactiveSubscription()
            .ExcludeFromDescription();

        return endpoints;
    }

    private static async Task<IResult> GetOnlinePaymentsAsync(
        IUseCase<GetOnlinePaymentsQuery, OnlinePaymentsResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetOnlinePaymentsQuery(), cancellationToken)).ToOkResult();

    private static async Task<IResult> StartOnboardingAsync(
        IUseCase<StartOnlinePaymentOnboardingCommand, OnlinePaymentRedirectResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new StartOnlinePaymentOnboardingCommand(), cancellationToken)).ToOkResult();

    private static async Task<IResult> RefreshAsync(
        IUseCase<RefreshOnlinePaymentAccountCommand, OnlinePaymentsResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new RefreshOnlinePaymentAccountCommand(), cancellationToken)).ToOkResult();

    private static async Task<IResult> UseAccountAsync(
        UseOnlinePaymentAccountCommand command,
        IUseCase<UseOnlinePaymentAccountCommand, OnlinePaymentsResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(command, cancellationToken)).ToOkResult();

    private static async Task<IResult> StopUsingAccountAsync(
        IUseCase<StopUsingOnlinePaymentAccountCommand, OnlinePaymentsResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new StopUsingOnlinePaymentAccountCommand(), cancellationToken)).ToOkResult();

    private static async Task<IResult> ReceiveStripeEventAsync(
        HttpRequest request,
        IOnlinePaymentWebhookReader webhookReader,
        IUseCase<SyncOnlinePaymentAccountCommand, bool> syncAccount,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(request.Body);
        var payload = await reader.ReadToEndAsync(cancellationToken);
        var onlinePaymentEvent = webhookReader.Read(payload, request.Headers[StripeSignatureHeaderName].ToString());
        if (onlinePaymentEvent is null)
        {
            return TypedResults.BadRequest();
        }

        if (onlinePaymentEvent.UpdatedExternalAccountId is { } externalAccountId)
        {
            var result = await syncAccount.ExecuteAsync(new SyncOnlinePaymentAccountCommand(externalAccountId), cancellationToken);
            if (result.IsFailure)
            {
                return result.ToOkResult();
            }
        }

        return TypedResults.Ok();
    }
}
