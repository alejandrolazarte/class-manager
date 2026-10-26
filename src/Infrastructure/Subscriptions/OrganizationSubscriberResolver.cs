using ClassManager.Core.Abstractions.Security;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Infrastructure.Subscriptions;

internal sealed class OrganizationSubscriberResolver(
    IBusinessRepository businessRepository,
    ICurrentUser currentUser,
    ITenantContext tenantContext) : ISubscriberResolver
{
    public async Task<long?> ResolveCurrentSubscriberIdAsync(CancellationToken cancellationToken) =>
        currentUser.OrganizationId ?? await businessRepository.FindOrganizationIdAsync(tenantContext.TenantId, cancellationToken);
}
