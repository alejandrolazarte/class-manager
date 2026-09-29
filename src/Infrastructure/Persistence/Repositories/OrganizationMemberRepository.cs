namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class OrganizationMemberRepository(AppDbContext context) : IOrganizationMemberRepository
{
    public void Add(OrganizationMember member) => context.OrganizationMembers.Add(member);

    public void Remove(OrganizationMember member) => context.OrganizationMembers.Remove(member);

    public Task<OrganizationMember?> FindForUpdateAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken) =>
        context.OrganizationMembers.FirstOrDefaultAsync(
            member => member.OrganizationId == organizationId && member.UserId == userId && member.Role == OrganizationRole.BrandOwner,
            cancellationToken);

    public async Task<IReadOnlyList<Guid>> ListBrandOwnerUserIdsAsync(Guid organizationId, CancellationToken cancellationToken) =>
        await context.OrganizationMembers
            .AsNoTracking()
            .Where(member => member.OrganizationId == organizationId && member.Role == OrganizationRole.BrandOwner)
            .Select(member => member.UserId)
            .ToListAsync(cancellationToken);
}
