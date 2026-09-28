namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class OrganizationRepository(AppDbContext context) : IOrganizationRepository
{
    public void Add(Organization organization) => context.Organizations.Add(organization);
}
