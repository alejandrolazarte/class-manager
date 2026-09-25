using ClassManager.Tenancy;

namespace ClassManager.Api.I.Tests.Infrastructure;

internal sealed class FixedTenantContext(Guid businessId) : ITenantContext
{
    public Guid TenantId => businessId;
}
