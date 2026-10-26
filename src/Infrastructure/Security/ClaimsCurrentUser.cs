using System.Globalization;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Infrastructure.Security;

internal sealed class ClaimsCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? AccountId =>
        Guid.TryParse(ClaimValue(SecurityClaimTypes.Subject), out var accountId) ? accountId : null;

    public long? UserId =>
        long.TryParse(ClaimValue(SessionClaimTypes.UserId), NumberStyles.None, CultureInfo.InvariantCulture, out var userId) ? userId : null;

    public long? OrganizationId =>
        long.TryParse(ClaimValue(SessionClaimTypes.OrganizationId), NumberStyles.None, CultureInfo.InvariantCulture, out var organizationId)
            ? organizationId
            : null;

    private string? ClaimValue(string claimType) => httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;
}
