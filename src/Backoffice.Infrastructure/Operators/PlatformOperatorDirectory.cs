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
        var memberships = await PlatformMembers()
            .AsNoTracking()
            .Where(member => member.User.AccountId == accountId)
            .Where(member => member.Role == BusinessRole.PlatformOperator || member.Role == BusinessRole.PlatformReader)
            .Select(member => new PlatformOperator(accountId, member.UserId, member.TenantId, member.Role))
            .ToListAsync(cancellationToken);

        return memberships
            .OrderBy(membership => membership.Role == BusinessRole.PlatformOperator ? 0 : 1)
            .FirstOrDefault();
    }

    public async Task<IReadOnlyList<PlatformTeamMember>> ListTeamAsync(CancellationToken cancellationToken) =>
        await PlatformMembers()
            .AsNoTracking()
            .Join(context.Businesses, member => member.TenantId, business => business.Id, (member, business) => new PlatformTeamMember(
                member.Id,
                business.Id,
                business.Name,
                member.UserId,
                member.User.AccountId,
                member.User.FullName,
                member.User.Email,
                member.Role))
            .ToListAsync(cancellationToken);

    public Task<BusinessMember?> FindTeamMemberAsync(long memberId, CancellationToken cancellationToken) =>
        PlatformMembers()
            .Include(member => member.User)
            .FirstOrDefaultAsync(member => member.Id == memberId, cancellationToken);

    private IQueryable<BusinessMember> PlatformMembers()
    {
        var platformOrganizationId = options.Value.PlatformOrganizationId;
        var platformBusinessIds = context.Businesses
            .Where(business => platformOrganizationId != null && business.OrganizationId == platformOrganizationId)
            .Select(business => business.Id);
        return context.BusinessMembers
            .IgnoreTenantFilter()
            .Where(member => platformBusinessIds.Contains(member.TenantId));
    }
}
