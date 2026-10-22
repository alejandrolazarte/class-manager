using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IBusinessMemberRepository
{
    void Add(BusinessMember member);

    void Remove(BusinessMember member);

    Task<IReadOnlyList<BusinessMember>> ListAsync(CancellationToken cancellationToken);

    Task<BusinessMember?> GetForUpdateAsync(long memberId, CancellationToken cancellationToken);

    Task<bool> IsAccountMemberAsync(Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<BusinessMember>> ListByCustomRoleAsync(long customRoleId, CancellationToken cancellationToken);

    Task<bool> IsInstructorLinkedAsync(long instructorId, long? exceptMemberId, CancellationToken cancellationToken);

    Task<Guid?> FindAccountIdByInstructorAsync(long instructorId, CancellationToken cancellationToken);
}
