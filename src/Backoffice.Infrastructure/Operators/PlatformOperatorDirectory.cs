using ClassManager.Backoffice.Core.Operators;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClassManager.Backoffice.Infrastructure.Operators;

internal sealed class PlatformOperatorDirectory(AppDbContext context, IOptions<BackofficeOptions> options) : IPlatformOperatorDirectory
{
    public async Task<PlatformOperator?> FindAsync(Guid accountId, CancellationToken cancellationToken)
    {
        if (options.Value.PlatformOrganizationId is not { } platformOrganizationId)
        {
            return null;
        }

        var platformBusinessIds = context.Businesses
            .Where(business => business.OrganizationId == platformOrganizationId)
            .Select(business => business.Id);
        var memberships = await context.BusinessMembers
            .IgnoreTenantFilter()
            .AsNoTracking()
            .Where(member => member.User.AccountId == accountId)
            .Where(member => member.Role == BusinessRole.PlatformOperator || member.Role == BusinessRole.PlatformReader)
            .Where(member => platformBusinessIds.Contains(member.TenantId))
            .Select(member => new PlatformOperator(accountId, member.UserId, member.TenantId, member.Role))
            .ToListAsync(cancellationToken);

        return memberships
            .OrderBy(membership => membership.Role == BusinessRole.PlatformOperator ? 0 : 1)
            .FirstOrDefault();
    }
}
