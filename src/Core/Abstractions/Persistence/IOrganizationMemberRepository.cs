using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IOrganizationMemberRepository
{
    void Add(OrganizationMember member);

    void Remove(OrganizationMember member);

    Task<OrganizationMember?> FindForUpdateAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Guid>> ListBrandOwnerUserIdsAsync(Guid organizationId, CancellationToken cancellationToken);
}
