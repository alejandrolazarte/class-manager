using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IOrganizationMemberRepository
{
    void Add(OrganizationMember member);

    void Remove(OrganizationMember member);

    Task<OrganizationMember?> FindForUpdateAsync(long organizationId, Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Guid>> ListBrandOwnerUserIdsAsync(long organizationId, CancellationToken cancellationToken);

    Task<bool> IsTheOnlyBrandOwnerOfABrandAsync(Guid accountId, CancellationToken cancellationToken);
}
