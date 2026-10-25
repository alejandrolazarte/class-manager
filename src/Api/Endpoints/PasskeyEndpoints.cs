using ClassManager.Api.Audit;
using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Passkeys;
using ClassManager.Infrastructure.Audit;
using ClassManager.Security.Hosting;

namespace ClassManager.Api.Endpoints;

internal static class PasskeyEndpoints
{
    public static IEndpointRouteBuilder MapPasskeyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var myPasskeys = endpoints.MapGroup(ApiRoutes.MyPasskeys).RequireAnyAccount();
        myPasskeys.MapGet(string.Empty, ListMyPasskeysAsync);
        myPasskeys.MapPost(ApiRoutes.PasskeyRegistrationOptions, BeginRegistrationAsync)
            .RequireRateLimiting(SecurityServiceCollectionExtensions.AuthenticationRateLimitPolicy);
        myPasskeys.MapPost(string.Empty, FinishRegistrationAsync)
            .RequireRateLimiting(SecurityServiceCollectionExtensions.AuthenticationRateLimitPolicy);
        myPasskeys.MapDelete(ApiRoutes.PasskeyById, RemoveMyPasskeyAsync);

        var authentication = endpoints.MapGroup(ApiRoutes.Authentication)
            .AllowAnonymous()
            .RequireRateLimiting(SecurityServiceCollectionExtensions.AuthenticationRateLimitPolicy);
        authentication.MapPost(ApiRoutes.PasskeySignInOptions, BeginSignInAsync);
        authentication.MapPost(ApiRoutes.PasskeySignIn, SignInAsync)
            .WithMetadata(new SecurityEventOnRejection(SecurityEventType.SignInFailed));

        return endpoints;
    }

    private static async Task<IResult> ListMyPasskeysAsync(
        IUseCase<ListMyPasskeysQuery, IReadOnlyList<PasskeyResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListMyPasskeysQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> BeginRegistrationAsync(
        BeginPasskeyRegistrationCommand command,
        IUseCase<BeginPasskeyRegistrationCommand, PasskeyCeremonyResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> FinishRegistrationAsync(
        FinishPasskeyRegistrationCommand command,
        IUseCase<FinishPasskeyRegistrationCommand, PasskeyResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(passkey => TypedResults.Created(ApiRoutes.MyPasskeys, passkey));
    }

    private static async Task<IResult> RemoveMyPasskeyAsync(
        string passkeyId,
        IUseCase<RemoveMyPasskeyCommand, RemoveMyPasskeyResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoveMyPasskeyCommand(passkeyId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> BeginSignInAsync(
        IUseCase<BeginPasskeySignInCommand, PasskeyCeremonyResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new BeginPasskeySignInCommand(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SignInAsync(
        SignInWithPasskeyCommand command,
        HttpContext httpContext,
        IUseCase<SignInWithPasskeyCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(tokens => TypedResults.Ok(RefreshTokenCookie.Keep(httpContext, tokens)));
    }
}
