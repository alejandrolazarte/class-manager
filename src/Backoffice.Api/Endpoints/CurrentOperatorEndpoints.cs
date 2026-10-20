using System.Security.Claims;
using ClassManager.Security.Tokens;

namespace ClassManager.Backoffice.Api.Endpoints;

internal static class CurrentOperatorEndpoints
{
    public static IEndpointRouteBuilder MapCurrentOperatorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(BackofficeApiRoutes.CurrentOperator, (ClaimsPrincipal user) => Results.Ok(new CurrentOperatorResponse(
            user.FindFirst(SecurityClaimTypes.Email)?.Value ?? string.Empty,
            user.FindFirst(SecurityClaimTypes.Role)?.Value ?? string.Empty)));

        return endpoints;
    }
}

internal sealed record CurrentOperatorResponse(string Email, string Role);
