using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;

namespace ClassManager.Api.Authentication;

internal sealed class ClaimsCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? AccountId => ClaimAsGuid(SecurityClaimTypes.Subject);

    public Guid? UserId => ClaimAsGuid(SessionClaimTypes.UserId);

    private Guid? ClaimAsGuid(string claimType) =>
        Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value, out var value)
            ? value
            : null;
}
