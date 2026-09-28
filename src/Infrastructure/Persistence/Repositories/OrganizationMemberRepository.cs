namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class OrganizationMemberRepository(AppDbContext context) : IOrganizationMemberRepository
{
    public void Add(OrganizationMember member) => context.OrganizationMembers.Add(member);
}
