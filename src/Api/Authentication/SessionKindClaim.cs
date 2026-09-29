using System.Security.Claims;
using ClassManager.Security.Tokens;

namespace ClassManager.Api.Authentication;

internal static class SessionKindClaim
{
    public static string? Of(ClaimsPrincipal user) => user.FindFirst(SecurityClaimTypes.Kind)?.Value;
}
