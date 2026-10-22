using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Tenancy.AspNetCore.Claims;

internal sealed class ClaimsTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext, ITenantScope
{
    private const string MissingTenantClaimMessage = "The access token has no valid tenant_id claim.";
    private const string TenantAlreadyEstablishedMessage = "Another tenant is already established for this request.";

    private long? _establishedTenantId;

    public long TenantId => _establishedTenantId ?? ReadTenantIdClaim();

    public void Establish(long tenantId)
    {
        if (_establishedTenantId is not null && _establishedTenantId != tenantId)
        {
            throw new InvalidOperationException(TenantAlreadyEstablishedMessage);
        }

        _establishedTenantId = tenantId;
    }

    private long ReadTenantIdClaim()
    {
        var claimValue = httpContextAccessor.HttpContext?.User.FindFirst(TenantClaimTypes.TenantId)?.Value;

        return long.TryParse(claimValue, NumberStyles.None, CultureInfo.InvariantCulture, out var tenantId) && tenantId > 0
            ? tenantId
            : throw new TenantNotResolvedException(MissingTenantClaimMessage);
    }
}
