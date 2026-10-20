using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Backoffice.Api.I.Tests.Infrastructure;

internal sealed class FakeGoogleHandler : IAuthenticationHandler
{
    public const string SubjectParameter = "sub";
    public const string EmailParameter = "email";

    private const string ExternalScheme = "backoffice-external";
    private const string AuthenticationType = "fake-google";

    private HttpContext? _context;

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        _context = context;
        return Task.CompletedTask;
    }

    public Task<AuthenticateResult> AuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());

    public async Task ChallengeAsync(AuthenticationProperties? properties)
    {
        var context = _context!;
        var identity = new ClaimsIdentity(
            [
                new Claim(SubjectParameter, context.Request.Query[SubjectParameter].ToString()),
                new Claim(EmailParameter, context.Request.Query[EmailParameter].ToString()),
            ],
            AuthenticationType);
        await context.SignInAsync(ExternalScheme, new ClaimsPrincipal(identity), properties);
        context.Response.Redirect(properties!.RedirectUri!);
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        _context!.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }
}
