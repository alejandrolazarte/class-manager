using Microsoft.AspNetCore.Http;

namespace ClassManager.Tenancy.AspNetCore.Claims;

internal sealed class ClaimsTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext, ITenantScope
{
    private const string MissingTenantClaimMessage = "The access token has no valid tenant_id claim.";
    private const string TenantAlreadyEstablishedMessage = "Another tenant is already established for this request.";

    private Guid? _establishedTenantId;

    public Guid TenantId => _establishedTenantId ?? ReadTenantIdClaim();

    public void Establish(Guid tenantId)
    {
        if (_establishedTenantId is not null && _establishedTenantId != tenantId)
        {
            throw new InvalidOperationException(TenantAlreadyEstablishedMessage);
        }

        _establishedTenantId = tenantId;
    }

    private Guid ReadTenantIdClaim()
    {
        var claimValue = httpContextAccessor.HttpContext?.User.FindFirst(TenantClaimTypes.TenantId)?.Value;

        return Guid.TryParse(claimValue, out var tenantId) && tenantId != Guid.Empty
            ? tenantId
            : throw new TenantNotResolvedException(MissingTenantClaimMessage);
    }
}
