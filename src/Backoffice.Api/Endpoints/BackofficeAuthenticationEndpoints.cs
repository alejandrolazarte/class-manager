using ClassManager.Backoffice.Api.Authentication;
using ClassManager.Backoffice.Core.UseCases;
using ClassManager.Backoffice.Infrastructure;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Security.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ClassManager.Backoffice.Api.Endpoints;

internal static class BackofficeAuthenticationEndpoints
{
    private const string CodeExtension = "code";

    public static IEndpointRouteBuilder MapBackofficeAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BackofficeApiRoutes.Authentication)
            .AllowAnonymous()
            .RequireRateLimiting(SecurityServiceCollectionExtensions.AuthenticationRateLimitPolicy);

        group.MapGet(BackofficeApiRoutes.GoogleSignIn, () => Results.Challenge(
            new AuthenticationProperties { RedirectUri = BackofficeApiRoutes.Authentication + BackofficeApiRoutes.GoogleSignInComplete },
            [BackofficeAuthenticationSchemes.Google]));
        group.MapGet(BackofficeApiRoutes.GoogleSignInComplete, CompleteGoogleSignInAsync);
        group.MapPost(BackofficeApiRoutes.Refresh, RefreshAsync);
        group.MapPost(BackofficeApiRoutes.SignOut, SignOutAsync);

        return endpoints;
    }

    private static async Task<IResult> CompleteGoogleSignInAsync(
        HttpContext httpContext,
        IUseCase<SignInOperatorCommand, OperatorSignInResponse> useCase,
        IOptions<BackofficeOptions> options,
        CancellationToken cancellationToken)
    {
        var external = await httpContext.AuthenticateAsync(BackofficeAuthenticationSchemes.External);
        await httpContext.SignOutAsync(BackofficeAuthenticationSchemes.External);
        var subject = external.Principal?.FindFirst(BackofficeAuthenticationSchemes.SubjectClaim)?.Value;
        var email = external.Principal?.FindFirst(BackofficeAuthenticationSchemes.EmailClaim)?.Value;
        var webAppUrl = options.Value.WebAppUrl.TrimEnd('/');
        if (!external.Succeeded || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(email))
        {
            return Results.Redirect(webAppUrl + BackofficeApiRoutes.WebAppSignIn);
        }

        var signIn = await useCase.ExecuteAsync(
            new SignInOperatorCommand(BackofficeAuthenticationSchemes.Google, subject, email), cancellationToken);
        if (signIn.Value?.Tokens is { } tokens)
        {
            BackofficeRefreshTokenCookie.Keep(httpContext.Response, tokens);
            return Results.Redirect(webAppUrl);
        }

        return Results.Redirect(signIn.Value?.AccessRequestCode is { } code
            ? $"{webAppUrl}{BackofficeApiRoutes.WebAppSignIn}?{BackofficeApiRoutes.AccessRequestCodeParameter}={Uri.EscapeDataString(code)}"
            : webAppUrl + BackofficeApiRoutes.WebAppSignIn);
    }

    private static async Task<IResult> RefreshAsync(
        HttpContext httpContext,
        IUseCase<RefreshSessionCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var refreshToken = BackofficeRefreshTokenCookie.IsRequested(httpContext.Request)
            ? BackofficeRefreshTokenCookie.Of(httpContext.Request)
            : null;
        var refreshed = await useCase.ExecuteAsync(new RefreshSessionCommand(refreshToken), cancellationToken);
        if (refreshed.Value is not { } tokens)
        {
            BackofficeRefreshTokenCookie.Forget(httpContext.Response);
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: AuthenticationErrorCodes.InvalidRefreshTokenMessage,
                extensions: new Dictionary<string, object?> { [CodeExtension] = AuthenticationErrorCodes.InvalidRefreshToken });
        }

        BackofficeRefreshTokenCookie.Keep(httpContext.Response, tokens);
        return Results.Ok(new BackofficeAccessTokenResponse(tokens.AccessToken, tokens.AccessTokenExpiresAt));
    }

    private static async Task<IResult> SignOutAsync(
        HttpContext httpContext,
        IUseCase<SignOutCommand, SignOutResponse> useCase,
        CancellationToken cancellationToken)
    {
        if (BackofficeRefreshTokenCookie.IsRequested(httpContext.Request))
        {
            await useCase.ExecuteAsync(new SignOutCommand(BackofficeRefreshTokenCookie.Of(httpContext.Request)), cancellationToken);
            BackofficeRefreshTokenCookie.Forget(httpContext.Response);
        }

        return Results.NoContent();
    }
}

internal sealed record BackofficeAccessTokenResponse(string AccessToken, DateTimeOffset AccessTokenExpiresAt);
