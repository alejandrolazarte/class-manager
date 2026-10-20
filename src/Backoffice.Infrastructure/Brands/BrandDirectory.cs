using ClassManager.Backoffice.Core.Brands;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Organizations;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Records;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Backoffice.Infrastructure.Brands;

internal sealed class BrandDirectory(AppDbContext context, TimeProvider timeProvider) : IBrandDirectory
{
    public async Task<IReadOnlyList<BrandListing>> ListAsync(CancellationToken cancellationToken)
    {
        var organizations = await context.Organizations.AsNoTracking().ToListAsync(cancellationToken);
        var ownersByBrand = (await OwnersOf(context.OrganizationMembers).ToListAsync(cancellationToken))
            .ToLookup(owner => owner.OrganizationId);
        var subscriptionsByBrand = (await context.Subscriptions.AsNoTracking().WhereNotDeleted().ToListAsync(cancellationToken))
            .ToDictionary(subscription => subscription.SubscriberId);
        var branchCounts = await context.Businesses
            .GroupBy(business => business.OrganizationId)
            .Select(branches => new { OrganizationId = branches.Key, Count = branches.Count() })
            .ToDictionaryAsync(count => count.OrganizationId, count => count.Count, cancellationToken);
        var studentCounts = await context.Students.IgnoreTenantFilter()
            .Join(context.Businesses, student => student.TenantId, business => business.Id, (_, business) => business.OrganizationId)
            .GroupBy(organizationId => organizationId)
            .Select(students => new { OrganizationId = students.Key, Count = students.Count() })
            .ToDictionaryAsync(count => count.OrganizationId, count => count.Count, cancellationToken);

        return [.. organizations.Select(organization => new BrandListing(
            organization.Id,
            organization.Name,
            organization.CreatedAt,
            ownersByBrand[organization.Id].Select(owner => owner.Owner).FirstOrDefault(),
            subscriptionsByBrand.GetValueOrDefault(organization.Id),
            branchCounts.GetValueOrDefault(organization.Id),
            studentCounts.GetValueOrDefault(organization.Id)))];
    }

    public async Task<BrandRecord?> FindAsync(Guid brandId, CancellationToken cancellationToken)
    {
        var organization = await context.Organizations.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == brandId, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        var owners = await OwnersOf(context.OrganizationMembers.Where(member => member.OrganizationId == brandId))
            .ToListAsync(cancellationToken);
        var subscriptions = await context.Subscriptions.AsNoTracking().IncludeDeleted()
            .Where(subscription => subscription.SubscriberId == brandId)
            .ToListAsync(cancellationToken);
        var current = subscriptions.FirstOrDefault(subscription => !subscription.IsDeleted);
        var addOns = current is null ? [] : await context.SubscriptionFeatures.AsNoTracking().WhereNotDeleted()
            .Where(addOn => addOn.SubscriptionId == current.Id)
            .ToListAsync(cancellationToken);
        var planFeatures = current is null ? [] : await context.PlanFeatures.AsNoTracking()
            .Where(planFeature => planFeature.PlanCode == current.PlanCode)
            .ToListAsync(cancellationToken);

        return new BrandRecord(
            organization.Id,
            organization.Name,
            organization.CreatedAt,
            [.. owners.Select(owner => owner.Owner)],
            await BranchesOfAsync(brandId, cancellationToken),
            subscriptions,
            addOns,
            planFeatures);
    }

    public async Task<IReadOnlyList<ExpiringAddOn>> ListAddOnsExpiringBetweenAsync(DateOnly from, DateOnly until, CancellationToken cancellationToken) =>
        await context.SubscriptionFeatures.AsNoTracking().WhereNotDeleted()
            .Where(addOn => addOn.ExpiredOn >= from && addOn.ExpiredOn <= until)
            .Join(context.Subscriptions.WhereNotDeleted(), addOn => addOn.SubscriptionId, subscription => subscription.Id, (addOn, subscription) => new { addOn, subscription.SubscriberId })
            .Join(context.Organizations, row => row.SubscriberId, organization => organization.Id, (row, organization) => new ExpiringAddOn(organization.Id, organization.Name, row.addOn.FeatureCode, row.addOn.ExpiredOn!.Value))
            .ToListAsync(cancellationToken);

    private IQueryable<OwnerRow> OwnersOf(IQueryable<OrganizationMember> members) =>
        members
            .Where(member => member.Role == OrganizationRole.BrandOwner)
            .Join(
                context.Users.IgnoreTenantFilter(),
                member => member.UserId,
                user => user.Id,
                (member, user) => new { member.OrganizationId, MemberId = member.Id, user.FullName, user.Email })
            .OrderBy(owner => owner.MemberId)
            .Select(owner => new OwnerRow(owner.OrganizationId, new BrandOwner(owner.FullName, owner.Email)));

    private async Task<IReadOnlyList<BrandBranch>> BranchesOfAsync(Guid brandId, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var branches = await context.Businesses.AsNoTracking()
            .Where(business => business.OrganizationId == brandId)
            .Select(business => new { business.Id, business.Name, business.CountryCode, business.CreatedAt })
            .ToListAsync(cancellationToken);
        var branchIds = branches.Select(branch => branch.Id).ToList();

        var students = await CountByBranchAsync(context.Students.IgnoreTenantFilter().Select(student => student.TenantId), branchIds, cancellationToken);
        var members = await CountByBranchAsync(
            context.BusinessMembers.IgnoreTenantFilter()
                .Where(member => member.Role != BusinessRole.BranchOwner)
                .Select(member => member.TenantId),
            branchIds,
            cancellationToken);
        var pendingInvitations = await CountByBranchAsync(
            context.MemberInvitations.IgnoreTenantFilter()
                .Where(invitation => invitation.Role != BusinessRole.BranchOwner
                    && invitation.AcceptedAt == null
                    && invitation.RevokedAt == null
                    && invitation.DeclinedAt == null
                    && invitation.ExpiresAt > now)
                .Select(invitation => invitation.TenantId),
            branchIds,
            cancellationToken);
        var documentBytes = await context.Documents.IgnoreTenantFilter()
            .Where(document => branchIds.Contains(document.TenantId))
            .GroupBy(document => document.TenantId)
            .Select(documents => new { BranchId = documents.Key, Bytes = documents.Sum(document => document.SizeInBytes) })
            .ToDictionaryAsync(sum => sum.BranchId, sum => (long)sum.Bytes, cancellationToken);

        return [.. branches.Select(branch => new BrandBranch(
            branch.Id,
            branch.Name,
            branch.CountryCode,
            branch.CreatedAt,
            students.GetValueOrDefault(branch.Id),
            members.GetValueOrDefault(branch.Id) + pendingInvitations.GetValueOrDefault(branch.Id),
            documentBytes.GetValueOrDefault(branch.Id)))];
    }

    private static Task<Dictionary<Guid, int>> CountByBranchAsync(IQueryable<Guid> branchIdsOfRows, List<Guid> branchIds, CancellationToken cancellationToken) =>
        branchIdsOfRows
            .Where(branchId => branchIds.Contains(branchId))
            .GroupBy(branchId => branchId)
            .Select(rows => new { BranchId = rows.Key, Count = rows.Count() })
            .ToDictionaryAsync(count => count.BranchId, count => count.Count, cancellationToken);

    private sealed record OwnerRow(Guid OrganizationId, BrandOwner Owner);
}
