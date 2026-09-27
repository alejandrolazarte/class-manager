using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Security.Hosting;

namespace ClassManager.Api.Endpoints;

internal static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authentication = endpoints.MapGroup(ApiRoutes.Authentication)
            .AllowAnonymous()
            .RequireRateLimiting(SecurityServiceCollectionExtensions.AuthenticationRateLimitPolicy);

        authentication.MapPost(ApiRoutes.SignUp, SignUpAsync);
        authentication.MapPost(ApiRoutes.SignIn, SignInAsync);
        authentication.MapPost(ApiRoutes.Refresh, RefreshAsync);
        authentication.MapPost(ApiRoutes.SignOut, SignOutAsync);
        authentication.MapPost(ApiRoutes.PasswordReset, ResetPasswordAsync);

        return endpoints;
    }

    private static async Task<IResult> SignUpAsync(
        SignUpOwnerCommand command,
        IUseCase<SignUpOwnerCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(tokens => TypedResults.Created(ApiRoutes.Business, tokens));
    }

    private static async Task<IResult> SignInAsync(
        SignInCommand command,
        IUseCase<SignInCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RefreshAsync(
        RefreshSessionCommand command,
        IUseCase<RefreshSessionCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SignOutAsync(
        SignOutCommand command,
        IUseCase<SignOutCommand, SignOutResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> ResetPasswordAsync(
        ResetPasswordCommand command,
        IUseCase<ResetPasswordCommand, ResetPasswordResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
