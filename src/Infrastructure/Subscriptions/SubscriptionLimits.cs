using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Subscriptions.AspNetCore.Limits;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Infrastructure.Subscriptions;

internal static class SubscriptionLimits
{
    public static void Configure(FeatureLimits<AppDbContext> limits)
    {
        limits.Count<Student>(
            Features.Students,
            (context, _, cancellationToken) => context.Students.CountAsync(cancellationToken));

        limits.Count<MemberInvitation>(
            Features.Team,
            CountTeamAsync,
            invitation => invitation.Role != BusinessRole.BranchOwner);

        limits.Count<Business>(
            Features.Branches,
            CountBranchesOfCurrentOrganizationAsync);
    }

    private static async Task<int> CountTeamAsync(AppDbContext context, IServiceProvider services, CancellationToken cancellationToken)
    {
        var now = services.GetRequiredService<TimeProvider>().GetUtcNow();
        var memberCount = await context.BusinessMembers.CountAsync(member => member.Role != BusinessRole.BranchOwner, cancellationToken);
        var pendingInvitationCount = await context.MemberInvitations.CountAsync(
            invitation => invitation.Role != BusinessRole.BranchOwner
                && invitation.AcceptedAt == null
                && invitation.RevokedAt == null
                && invitation.DeclinedAt == null
                && invitation.ExpiresAt > now,
            cancellationToken);
        return memberCount + pendingInvitationCount;
    }

    private static async Task<int> CountBranchesOfCurrentOrganizationAsync(AppDbContext context, IServiceProvider services, CancellationToken cancellationToken)
    {
        var businessId = services.GetRequiredService<ITenantContext>().TenantId;
        var organizationId = await context.Businesses
            .Where(business => business.Id == businessId)
            .Select(business => business.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);
        return await context.Businesses.CountAsync(business => business.OrganizationId == organizationId, cancellationToken);
    }
}
