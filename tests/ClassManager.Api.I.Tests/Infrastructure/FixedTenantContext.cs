using ClassManager.Tenancy;

namespace ClassManager.Api.I.Tests.Infrastructure;

internal sealed class FixedTenantContext(long businessId) : ITenantContext
{
    public long TenantId => businessId;
}
