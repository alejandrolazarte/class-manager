using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Infrastructure.Security;

internal sealed class ClaimsCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? AccountId => ClaimAsGuid(SecurityClaimTypes.Subject);

    public Guid? UserId => ClaimAsGuid(SessionClaimTypes.UserId);

    private Guid? ClaimAsGuid(string claimType) =>
        Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value, out var value)
            ? value
            : null;
}
